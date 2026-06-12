using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using SillyTavernMCP;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddConsole(consoleLogOptions =>
{
    // Configure all logs to go to stderr
    consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
});
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
    httpClient.GetAsync("/csrf-token").GetAwaiter().GetResult(); // Pre-fetch CSRF token to validate connection and fail fast if misconfigured.
});
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
