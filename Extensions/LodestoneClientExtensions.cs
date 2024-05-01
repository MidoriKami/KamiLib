using System;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiLib.Configuration;
using NetStone;
using NetStone.Model.Parseables.Character;
using NetStone.Search.Character;

namespace KamiLib.Classes;

public static class LodestoneClientExtensions {
    internal static async Task<LodestoneCharacter?> TryGetLodestoneCharacter(this LodestoneClient client, DalamudPluginInterface pluginInterface, IPluginLog log, CharacterConfiguration character) {
        try {
            // If lodestone id is null, try and fetch it by searching for name and world.
            if (character.LodestoneId is null) {
                var searchResponse = await client.SearchCharacter(new CharacterSearchQuery {
                    CharacterName = character.CharacterName,
                    World = character.CharacterWorld,
                });

                character.LodestoneId = searchResponse
                    ?.Results
                    .FirstOrDefault(entry => string.Equals(entry.Name, character.CharacterName, StringComparison.OrdinalIgnoreCase))
                    ?.Id;
            
                pluginInterface.SaveCharacterFile(character.ContentId, "System.config.json", character);
            }

            // If it is still null, then we couldn't find it.
            if (character.LodestoneId is null) return null;
        
            // Use lodestoneId to get character.
            return await client.GetCharacter(character.LodestoneId);
        }
        catch (Exception e) {
            log.Error(e, $"Exception trying to get lodestone data for {character.CharacterName}@{character.CharacterWorld}");
            return null;
        }
    }
}