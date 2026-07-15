using System.IO;
using FluentAssertions;

namespace Guppi.Repl.Tests.Mcp
{

    [TestFixture]
    public sealed class McpModuleTests
    {
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
    }
}
