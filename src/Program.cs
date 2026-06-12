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
    httpClient.DefaultRequestHeaders.Accept.Add(new("application/json"));
    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SillyTavernMCP/1.0");

    if (!string.IsNullOrWhiteSpace(options.ApiKey))
    {
        httpClient.DefaultRequestHeaders.Add("X-API-KEY", options.ApiKey);
    }
});
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithTools<SillyTavernTools>();

await builder.Build().RunAsync();
