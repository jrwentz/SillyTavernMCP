using System.ComponentModel;
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

    [McpServerTool(Name = "get_sillytavern_settings")]
    [Description("Gets the current SillyTavern settings bundle as formatted JSON text.")]
    public Task<string> GetSettings() => _client.GetSettingsAsync();

    [McpServerTool(Name = "list_sillytavern_characters")]
    [Description("Lists characters available in the configured SillyTavern instance as formatted JSON text.")]
    public Task<string> ListCharacters() => _client.GetCharactersAsync();

    [McpServerTool(Name = "list_sillytavern_groups")]
    [Description("Lists groups available in the configured SillyTavern instance as formatted JSON text.")]
    public Task<string> ListGroups() => _client.GetGroupsAsync();
}
