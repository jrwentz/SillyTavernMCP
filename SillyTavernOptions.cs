namespace SillyTavernMCP;

public sealed class SillyTavernOptions
{
    private const string DefaultBaseUrl = "http://127.0.0.1:8000";

    public SillyTavernOptions(Uri baseUrl, string? apiKey)
    {
        BaseUrl = baseUrl;
        ApiKey = string.IsNullOrWhiteSpace(apiKey) ? null : apiKey.Trim();
    }

    public Uri BaseUrl { get; }

    public string? ApiKey { get; }

    public static SillyTavernOptions LoadFromEnvironment()
    {
        var baseUrl = Environment.GetEnvironmentVariable("SILLY_TAVERN_BASE_URL");
        var apiKey = Environment.GetEnvironmentVariable("SILLY_TAVERN_API_KEY");
        var normalizedBaseUrl = string.IsNullOrWhiteSpace(baseUrl) ? DefaultBaseUrl : baseUrl.Trim();

        if (!Uri.TryCreate(normalizedBaseUrl, UriKind.Absolute, out var parsedBaseUrl))
        {
            throw new InvalidOperationException(
                $"SILLY_TAVERN_BASE_URL must be an absolute URL (received: '{normalizedBaseUrl}'), for example http://127.0.0.1:8000.");
        }

        return new SillyTavernOptions(parsedBaseUrl, apiKey);
    }
}
