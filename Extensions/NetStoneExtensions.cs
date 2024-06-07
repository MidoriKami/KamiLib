using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Dalamud.Interface.Internal;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiLib.Configuration;
using NetStone;

namespace KamiLib.Extensions;

public static class NetStoneExtensions {
    internal static async Task<IDalamudTextureWrap?> TryGetProfilePicture(HttpClient httpClient, LodestoneClient lodestoneClient, DalamudPluginInterface pluginInterface, ITextureProvider textureProvider, IPluginLog log, CharacterConfiguration characterConfiguration) {
        try {
            // We had some error while loading character configuration and don't know what character this is.
            if (characterConfiguration.ContentId is 0) return null;

            var profilePictureFileInfo = pluginInterface.GetProfilePictureFileInfo(characterConfiguration);
        
            // If we already have a "profile.png" picture in this characters directory, simply load it.
            if (profilePictureFileInfo is { Exists: true } && !characterConfiguration.PurgeProfilePicture) {
                return textureProvider.GetFromFile(profilePictureFileInfo.FullName).GetWrapOrDefault();
            }

            // Else, try and get the LodestoneCharacter information that matches our name and world
            if (await lodestoneClient.TryGetLodestoneCharacter(pluginInterface, log, characterConfiguration) is { } lodestoneCharacter) {
                if (await httpClient.GetAsync(lodestoneCharacter.Portrait) is { } lodestoneRequestResponse) {
                    await using (var mediaStream = await lodestoneRequestResponse.Content.ReadAsStreamAsync()) {
                        await using (var fileStream = new FileStream(profilePictureFileInfo.FullName, FileMode.Create)) {
                            await mediaStream.CopyToAsync(fileStream);
                        }
                    }
                    
                    // using var, should make the filestream dispose and flush the contents, but that won't update the fileInfo
                    profilePictureFileInfo.Refresh();
                    
                    // Saving of "Profile.png" should be complete here, load the image.
                    return profilePictureFileInfo is { Exists: false } ? null : textureProvider.GetFromFile(profilePictureFileInfo.FullName).GetWrapOrDefault();
                }
            }
            // Else, unrecoverable, we don't have a "profile.png" and we couldn't load it from Lodestone.
            else {
                return null;
            }

            // Somehow slipped into unknown territory, return null.
            return null;
        }
        catch (Exception e) {
            log.Error(e, $"Exception trying to load profile picture for {characterConfiguration.CharacterName}@{characterConfiguration.CharacterWorld}");
            return null;
        }
    }
}