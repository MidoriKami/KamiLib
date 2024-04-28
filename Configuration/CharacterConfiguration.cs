using System.Text.Json.Serialization;
using Dalamud.Configuration;
using Dalamud.Plugin.Services;

namespace KamiLib.Configuration;

public class CharacterConfiguration  : IPluginConfiguration {
    public int Version { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public string CharacterWorld { get; set; } = string.Empty;
    public ulong ContentId { get; set; }
    public string? LodestoneId { get; set; }

    [JsonIgnore] public bool PurgeProfilePicture { get; set; }= false;

    public void UpdateCharacterData(IClientState clientState) {
        if (clientState is { LocalPlayer: { Name: var playerName, HomeWorld.GameData.Name: var homeWorld } }) {
            CharacterName = playerName.ToString();
            CharacterWorld = homeWorld.ToString();
        }
    }
}