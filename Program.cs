using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Server;
using SillyTavernMCP;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(_ => SillyTavernOptions.LoadFromEnvironment());
builder.Services.AddHttpClient<SillyTavernClient>((serviceProvider, httpClient) =>
{
    var options = serviceProvider.GetRequiredService<SillyTavernOptions>();
    httpClient.BaseAddress = options.BaseUrl;
});
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<SillyTavernTools>();

await builder.Build().RunAsync();
