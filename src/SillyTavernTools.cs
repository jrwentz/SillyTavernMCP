using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelContextProtocol.Server;

namespace SillyTavernMCP;

[McpServerToolType]
public sealed class SillyTavernTools
{
    private readonly SillyTavernClient _client;

    public SillyTavernTools(SillyTavernClient client)
    {
        _client = client;
    }

    //[McpServerTool(Name = "get_sillytavern_settings")]
    //[Description("Gets the current SillyTavern settings bundle as formatted JSON text.")]
    //public Task<string> GetSettings() => _client.GetSettingsAsync();

    [McpServerTool(Name = "list_sillytavern_characters", UseStructuredContent = true)]
    [Description("Lists characters available in the configured SillyTavern instance as formatted JSON text.")]
    public async Task<List<string>> ListCharacters()
    {
        var characters = await _client.GetCharactersAsync();
        using var document = JsonDocument.Parse(characters);
        var characterList = new List<string>();
        foreach (var character in document.RootElement.EnumerateArray())
        {
            var name = character.GetProperty("name").GetString();
            if(name != null)
            {
                characterList.Add(name);
            }
        }
        return characterList;
    }

    [McpServerTool(Name = "get_sillytavern_character")]
    [Description("Gets details for a character in the configured SillyTavern instance as formatted JSON text. Specify the character name as the argument.")]
    public async Task<string> GetCharacterDetails(string characterName)
    {
        var characters = await _client.GetCharactersAsync();
        using var document = JsonDocument.Parse(characters);
        foreach (var character in document.RootElement.EnumerateArray())
        {
            var name = character.GetProperty("name").GetString();
            if (name != null && name.Equals(characterName, StringComparison.OrdinalIgnoreCase))
            {
                return JsonSerializer.Serialize(character, new JsonSerializerOptions { WriteIndented = true });
            }
        }
        throw new InvalidOperationException($"Character '{characterName}' not found in SillyTavern.");
    }

    [McpServerTool(Name = "list_sillytavern_groups")]
    [Description("Lists groups available in the configured SillyTavern instance as formatted JSON text.")]
    public Task<string> ListGroups() => _client.GetGroupsAsync();

    [McpServerTool(Name = "list_sillytavern_lorebooks", UseStructuredContent = true)]
    [Description("Lists lorebooks available in the configured SillyTavern instance as formatted JSON list.")]
    public async Task<HashSet<string>> ListLorebooks()
    {
        var settingsJson = await _client.GetSettingsAsync();
        using var document = JsonDocument.Parse(settingsJson);
        var lorebooks = new HashSet<string>();
        if (document.RootElement.TryGetProperty("world_names", out var lorebooksElement))
        {
            foreach (var lorebook in lorebooksElement.EnumerateArray())
            {
                lorebooks.Add(lorebook.ToString());
            }
        }
        return lorebooks;
    }
}
