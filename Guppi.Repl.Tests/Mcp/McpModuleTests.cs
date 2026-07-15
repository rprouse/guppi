using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace Guppi.Repl.Tests.Mcp
{

    [TestFixture]
    public sealed class McpModuleTests
    {
        private static readonly string[] ApprovedCommandPaths =
        [
            "utilities date",
            "utilities guid",
            "ip local",
            "hue bridges",
            "hue lights",
            "hue {light} on",
            "hue {light} off",
        ];

        private static readonly string[] ApprovedToolNames =
        [
            "utilities_date",
            "utilities_guid",
            "ip_local",
            "hue_bridges",
            "hue_lights",
            "hue_on",
            "hue_off",
        ];

        [Test]
        public void McpCommandFilterAllowsOnlyApprovedPaths()
        {
            var filter = typeof(GuppiReplApp).GetMethod(
                "IsMcpCommandAllowed",
                BindingFlags.NonPublic | BindingFlags.Static);

            filter.Should().NotBeNull("the MCP exposure policy must be independently testable");
            ApprovedCommandPaths.Should().OnlyContain(path =>
                (bool)filter.Invoke(null, new object[] { path }));
            new[] { "hue register", "hue lights delete", "utilities secrets", "ip local admin" }
                .Should().OnlyContain(path => !(bool)filter.Invoke(null, new object[] { path }));
        }

        [Test]
        [NonParallelizable]
        public void McpServeCommandIsPresentOnCliGraph()
        {
            var originalOutput = Console.Out;
            var originalError = Console.Error;
            using var output = new StringWriter();

            int exitCode;
            try
            {
                Console.SetOut(output);
                Console.SetError(output);
                exitCode = GuppiReplApp.Create().Run(["mcp", "--help", "--no-logo"]);
            }
            finally
            {
                Console.SetOut(originalOutput);
                Console.SetError(originalError);
            }

            exitCode.Should().Be(0, output.ToString());
            output.ToString().Should().Contain("serve");
        }

        [Test]
        public async Task McpStdioExposesExactTypedGraphAndExecutesAUtility()
        {
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var server = StartMcpServer();
            var stderrTask = server.StandardError.ReadToEndAsync(timeout.Token);

            try
            {
                await SendAsync(server, new
                {
                    jsonrpc = "2.0",
                    id = 1,
                    method = "initialize",
                    @params = new
                    {
                        protocolVersion = "2025-06-18",
                        capabilities = new { },
                        clientInfo = new { name = "guppi-tests", version = "1" },
                    },
                });
                using var initialized = await ReadResponseAsync(server, 1, timeout.Token);
                initialized.RootElement.GetProperty("result")
                    .GetProperty("serverInfo").GetProperty("name").GetString()
                    .Should().Be("Guppi");

                await SendAsync(server, new
                {
                    jsonrpc = "2.0",
                    method = "notifications/initialized",
                    @params = new { },
                });
                await SendAsync(server, new
                {
                    jsonrpc = "2.0",
                    id = 2,
                    method = "tools/list",
                    @params = new { },
                });
                using var listed = await ReadResponseAsync(server, 2, timeout.Token);
                var tools = listed.RootElement.GetProperty("result").GetProperty("tools")
                    .EnumerateArray().ToArray();
                tools.Select(tool => tool.GetProperty("name").GetString())
                    .Should().BeEquivalentTo(ApprovedToolNames);

                var dateSchema = tools.Single(tool =>
                    tool.GetProperty("name").GetString() == "utilities_date")
                    .GetProperty("inputSchema");
                dateSchema.GetProperty("properties").GetProperty("utc")
                    .GetProperty("type").GetString().Should().Be("boolean");

                var hueOnSchema = tools.Single(tool =>
                    tool.GetProperty("name").GetString() == "hue_on")
                    .GetProperty("inputSchema");
                hueOnSchema.GetProperty("properties").GetProperty("brightness")
                    .GetProperty("type").GetString().Should().Be("integer");
                var hueOnAnnotations = tools.Single(tool =>
                    tool.GetProperty("name").GetString() == "hue_on")
                    .GetProperty("annotations");
                hueOnAnnotations.GetProperty("destructiveHint").GetBoolean().Should().BeTrue();
                hueOnAnnotations.GetProperty("idempotentHint").GetBoolean().Should().BeTrue();
                hueOnAnnotations.GetProperty("openWorldHint").GetBoolean().Should().BeTrue();

                await SendAsync(server, new
                {
                    jsonrpc = "2.0",
                    id = 3,
                    method = "tools/call",
                    @params = new
                    {
                        name = "utilities_date",
                        arguments = new { utc = true },
                    },
                });
                using var called = await ReadResponseAsync(server, 3, timeout.Token);
                var callResult = called.RootElement.GetProperty("result");
                callResult.TryGetProperty("isError", out var isError).Should().BeTrue();
                isError.GetBoolean().Should().BeFalse();
            }
            finally
            {
                server.StandardInput.Close();
                try
                {
                    await server.WaitForExitAsync(timeout.Token);
                }
                catch (OperationCanceledException)
                {
                    server.Kill(true);
                    await server.WaitForExitAsync();
                    throw;
                }
            }

            var stderr = await stderrTask;
            server.ExitCode.Should().Be(0, stderr);
        }

        private static Process StartMcpServer()
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = Path.GetDirectoryName(typeof(GuppiReplApp).Assembly.Location),
            };
            startInfo.ArgumentList.Add(typeof(GuppiReplApp).Assembly.Location);
            startInfo.ArgumentList.Add("mcp");
            startInfo.ArgumentList.Add("serve");
            startInfo.ArgumentList.Add("--no-logo");
            return Process.Start(startInfo)
                ?? throw new InvalidOperationException("Could not start the Guppi MCP test process.");
        }

        private static async Task SendAsync(Process server, object request)
        {
            await server.StandardInput.WriteLineAsync(JsonSerializer.Serialize(request));
            await server.StandardInput.FlushAsync();
        }

        private static async Task<JsonDocument> ReadResponseAsync(
            Process server,
            int requestId,
            CancellationToken cancellationToken)
        {
            while (true)
            {
                var line = await server.StandardOutput.ReadLineAsync(cancellationToken);
                if (line is null)
                {
                    var stderr = await server.StandardError.ReadToEndAsync(cancellationToken);
                    throw new AssertionException(
                        $"MCP server exited before response {requestId}. stderr: {stderr}");
                }

                JsonDocument message;
                try
                {
                    message = JsonDocument.Parse(line);
                }
                catch (JsonException exception)
                {
                    throw new AssertionException($"MCP stdout contained non-JSON data: {line}", exception);
                }

                if (message.RootElement.TryGetProperty("id", out var id)
                    && id.ValueKind == JsonValueKind.Number
                    && id.GetInt32() == requestId)
                {
                    return message;
                }

                message.Dispose();
            }
        }
    }
}
