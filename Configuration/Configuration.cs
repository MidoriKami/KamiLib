using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin;
using KamiLib.Configuration;

namespace KamiLib.Classes;

public static class Configuration {
    private static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true,
        IncludeFields = true,
    };

    private static CharacterConfiguration LoadCharacterConfiguration(this DalamudPluginInterface pluginInterface, ulong contentId) {
        var loadedConfiguration = pluginInterface.LoadCharacterFile(contentId, "System.config.json", () => CreateNewCharacterConfig(contentId));
        if (loadedConfiguration is { Version: not 2, ContentId: 0 }) {
            loadedConfiguration.Version = 2;
            loadedConfiguration.ContentId = contentId;
            pluginInterface.SaveCharacterFile(contentId, "System.config.json", loadedConfiguration);
        }

        return loadedConfiguration;
    }
    
    public static T LoadCharacterFile<T>(this DalamudPluginInterface pluginInterface, ulong contentId, string fileName, Func<T> createFunc) where T : new() {
        var fileInfo = pluginInterface.GetCharacterFileInfo(contentId, fileName);
        if (fileInfo is { Exists: true }) {
            try {
                var fileText = File.ReadAllText(fileInfo.FullName);
                var dataObject = JsonSerializer.Deserialize<T>(fileText, SerializerOptions);

                // If deserialize result is null, create a new instance instead and save it.
                if (dataObject is null) {
                    dataObject = createFunc();
                    pluginInterface.SaveCharacterFile(contentId, fileName, dataObject);
                }
                
                return dataObject;
            }
            catch (Exception) {
                // If there is any kind of error loading the file, generate a new one instead and save it.
                var dataObject = createFunc();
                pluginInterface.SaveCharacterFile(contentId, fileName, dataObject);
            }
        }
        
        return createFunc();
    }

    public static void SaveCharacterFile<T>(this DalamudPluginInterface pluginInterface, ulong contentId, string fileName, T file) {
        try {
            var fileInfo = pluginInterface.GetCharacterFileInfo(contentId, fileName);
            var fileText = JsonSerializer.Serialize(file, SerializerOptions);
            Dalamud.Utility.Util.WriteAllTextSafe(fileInfo.FullName, fileText);
        }
        catch (Exception e) {
            throw new Exception($"Error while trying to save file for CID:{contentId}, {fileName}.", e);
        }
    }
    
    public static DirectoryInfo GetCharacterDirectoryInfo(this DalamudPluginInterface pluginInterface, ulong contentId) 
        => new(Path.Combine(pluginInterface.ConfigDirectory.FullName, contentId.ToString()));

    public static FileInfo GetCharacterFileInfo(this DalamudPluginInterface pluginInterface, ulong contentId, string fileName) 
        => new(Path.Combine(pluginInterface.GetCharacterDirectoryInfo(contentId).FullName, fileName));

    internal static IEnumerable<CharacterConfiguration> GetAllCharacterConfigurations(this DalamudPluginInterface pluginInterface) 
        => pluginInterface.GetAllCharacterContentIds().Select(pluginInterface.LoadCharacterConfiguration);

    private static IEnumerable<ulong> GetAllCharacterContentIds(this DalamudPluginInterface pluginInterface) {
        if (pluginInterface.ConfigDirectory is { Exists: true } directoryInfo) {
            foreach (var directory in directoryInfo.EnumerateDirectories()) {
                if (ulong.TryParse(directory.Name, out var contentId)) {
                    yield return contentId;
                }
            }
        }
    }

    private static CharacterConfiguration CreateNewCharacterConfig(ulong contentId) 
        => new() {
            CharacterName = "Unknown Name",
            ContentId = contentId,
            Version = 2,
            CharacterWorld = "Unknown World",
        };
}