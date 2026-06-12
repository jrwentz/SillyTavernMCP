using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;
using SillyTavernMCP;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(_ => SillyTavernOptions.LoadFromEnvironment());
builder.Services.AddSingleton<SillyTavernClient>();
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<SillyTavernTools>();

await builder.Build().RunAsync();
