using System.IO;
using Dalamud.Plugin;
using KamiLib.Configuration;

namespace KamiLib.Classes;

public static class PluginInterfaceExtensions {
    internal static FileInfo GetProfilePictureFileInfo(this DalamudPluginInterface pluginInterface, CharacterConfiguration characterConfiguration) 
        => new(Path.Combine(pluginInterface.GetCharacterDirectoryInfo(characterConfiguration.ContentId).FullName, "profile.png"));
}