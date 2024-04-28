using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Dalamud.Interface.Internal;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiLib.Configuration;
using NetStone;

namespace KamiLib.Classes;

public static class NetStoneExtensions {
    public static async Task<IDalamudTextureWrap?> TryGetProfilePicture(HttpClient httpClient, LodestoneClient lodestoneClient, DalamudPluginInterface pluginInterface, ITextureProvider textureProvider, CharacterConfiguration characterConfiguration) {
        // We had some error while loading character configuration and don't know what character this is.
        if (characterConfiguration.ContentId is 0) return null;

        var profilePictureFileInfo = pluginInterface.GetProfilePictureFileInfo(characterConfiguration);
        
        // If we already have a "profile.png" picture in this characters directory, simply load it.
        if (profilePictureFileInfo is { Exists: true } && !characterConfiguration.PurgeProfilePicture) {
            return textureProvider.GetTextureFromFile(profilePictureFileInfo);
        }
        // Else we need to try and load it from Lodestone.
        else {
            // Try and get the LodestoneCharacter information that matches our name and world
            if (await lodestoneClient.TryGetLodestoneCharacter(pluginInterface, characterConfiguration) is { } lodestoneCharacter) {
                if (await httpClient.GetAsync(lodestoneCharacter.Portrait) is { } lodestoneRequestResponse) {
                    await using (var mediaStream = await lodestoneRequestResponse.Content.ReadAsStreamAsync()) {
                        await using (var fileStream = new FileStream(profilePictureFileInfo.FullName, FileMode.Create)) {
                            await mediaStream.CopyToAsync(fileStream);
                        }
                    }
                    
                    profilePictureFileInfo.Refresh();
                    
                    // Saving of "Profile.png" should be complete here, load the image.
                    return profilePictureFileInfo is { Exists: false } ? null : textureProvider.GetTextureFromFile(profilePictureFileInfo);
                }
            }
            // Else, unrecoverable, we don't have a "profile.png" and we couldn't load it from Lodestone.
            else {
                return null;
            }
        }
        
        // Somehow slipped into unknown territory, return null.
        return null;
    }
}