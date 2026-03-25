using Guppi.Core;
using Guppi.MCP.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddCore()
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<UtilitiesTools>();

await builder.Build().RunAsync();
