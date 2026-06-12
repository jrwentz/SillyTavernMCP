using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SillyTavernMCP;

public sealed class SillyTavernClient
{
    private static readonly JsonSerializerOptions JsonWriterOptions = new() { WriteIndented = true };
    private readonly HttpClient _httpClient;

    public SillyTavernClient(HttpClient httpClient, SillyTavernOptions options)
    {
        _httpClient = httpClient;

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SillyTavernMCP/1.0");

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", options.ApiKey);
        }
    }

    public Task<string> GetSettingsAsync() => PostAsync("/api/settings/get");

    public Task<string> GetCharactersAsync() => PostAsync("/api/characters/all");

    public Task<string> GetGroupsAsync() => PostAsync("/api/groups/all");

    private async Task<string> PostAsync(string relativePath)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, relativePath)
        {
            Content = new StringContent("{}", Encoding.UTF8, "application/json"),
        };

        using var response = await _httpClient.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"SillyTavern request to '{relativePath}' failed with status {(int)response.StatusCode}: {body}");
        }

        return TryFormatJson(body);
    }

    private static string TryFormatJson(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return string.Empty;
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
}
