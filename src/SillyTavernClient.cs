using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SillyTavernMCP;

public sealed class SillyTavernClient
{
    private const string EmptyJsonObject = "{}";
    private static readonly JsonSerializerOptions JsonWriterOptions = new() { WriteIndented = true };
    private readonly HttpClient _httpClient;
    private string _token = string.Empty;
    private ILogger<SillyTavernClient> _logger;

    public SillyTavernClient(HttpClient httpClient, ILogger<SillyTavernClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        Initialize();
    }

    public Task<string> GetSettingsAsync() => PostAsync("/api/settings/get");

    public Task<string> GetCharactersAsync() => PostAsync("/api/characters/all");

    public Task<string> GetGroupsAsync() => PostAsync("/api/groups/all");

    private async Task<string> PostAsync(string relativePath)
    {
        _logger.LogTrace($"Sending request to {relativePath}");
        using var request = new HttpRequestMessage(HttpMethod.Post, relativePath)
        {
            Content = new StringContent(EmptyJsonObject, Encoding.UTF8, "application/json"),
        };

        using var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"SillyTavern request to '{relativePath}' failed with status {(int)response.StatusCode} ({response.ReasonPhrase}).");
        }
        
        _logger.LogTrace($"Received response from {relativePath}: {body}");
        return TryFormatJson(body);
    }

    private static string TryFormatJson(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return EmptyJsonObject;
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            return JsonSerializer.Serialize(document.RootElement, JsonWriterOptions);
        }
        catch (JsonException)
        {
            return responseBody;
        }
    }

    internal void Initialize()
    {
        _logger.LogInformation("Initializing SillyTavernClient and retrieving CSRF token.");
        using var response = _httpClient.GetAsync("/csrf-token").GetAwaiter().GetResult();
        var tokenJson = JsonNode.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
        var token = tokenJson?["token"];
        _token = token?.ToString();
        _httpClient.DefaultRequestHeaders.Add("X-CSRF-Token", _token);
    }
}
