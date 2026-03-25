using Guppi.Core;
using Guppi.MCP.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

builder.Services
    .AddCore()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<UtilitiesTools>();

await builder.Build().RunAsync();
