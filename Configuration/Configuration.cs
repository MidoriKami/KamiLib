using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin;
using KamiLib.Classes;

namespace KamiLib.Configuration;

public static class Configuration {
    public static readonly JsonSerializerOptions SerializerOptions = new() {
        WriteIndented = true,
        IncludeFields = true,
    };
    
    /// <summary>
    /// Load a configuration file for a specific character.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface used to find the correct directory.</param>
    /// <param name="contentId">ContentId of the desired character file.</param>
    /// <param name="fileName">Specific name of the file you wish to load.</param>
    /// <param name="createFunc">A function to create that file if loading fails.</param>
    /// <typeparam name="T">Type of file to load, needed for serialization.</typeparam>
    /// <returns>Either a loaded config file, or a new'd file of type T.</returns>
    public static T LoadCharacterFile<T>(this DalamudPluginInterface pluginInterface, ulong contentId, string fileName, Func<T> createFunc) where T : new() 
        => LoadFile(pluginInterface, pluginInterface.GetCharacterFileInfo(contentId, fileName).FullName, createFunc);

    /// <summary>
    /// Load a configuration file from the plugin specific config directory.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface used to find the correct directory.</param>
    /// <param name="fileName">Specific name of the file you wish you load.</param>
    /// <param name="createFunc">A function to create the file if loading fails.</param>
    /// <typeparam name="T">Type of file to load, needed for serialization.</typeparam>
    /// <returns>Either a loaded config file, or a new'd file of type T.</returns>
    public static T LoadConfigFile<T>(this DalamudPluginInterface pluginInterface, string fileName, Func<T> createFunc) where T : new() 
        => LoadFile(pluginInterface, Path.Combine(pluginInterface.ConfigDirectory.FullName, fileName), createFunc);

    /// <summary>
    /// Save a configuration file for a specific character.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface used to find the correct directory.</param>
    /// <param name="contentId">ContentId of the desired character file.</param>
    /// <param name="fileName">Specific name of the file you wish to save.</param>
    /// <param name="file">The object to write to a file.</param>
    /// <typeparam name="T">Type of file to load, needed for serialization.</typeparam>
    public static void SaveCharacterFile<T>(this DalamudPluginInterface pluginInterface, ulong contentId, string fileName, T file) 
        => SaveFile(pluginInterface, pluginInterface.GetCharacterFileInfo(contentId, fileName).FullName, file);

    /// <summary>
    /// Save a configuration file to the plugin specific config directory.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface used to find the correct directory.</param>
    /// <param name="fileName">Specific name of the file you wish to save.</param>
    /// <param name="file">The object to write to a file.</param>
    /// <typeparam name="T">Type of file to load, needed for serialization.</typeparam>
    public static void SaveConfigFile<T>(this DalamudPluginInterface pluginInterface, string fileName, T file)
        => SaveFile(pluginInterface, Path.Combine(pluginInterface.GetPluginConfigDirectory(), fileName), file);

    private static T LoadFile<T>(DalamudPluginInterface pluginInterface, string filePath, Func<T> createFunc) where T : new() {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo is { Exists: true }) {
            try {
                var fileText = File.ReadAllText(fileInfo.FullName);
                var dataObject = JsonSerializer.Deserialize<T>(fileText, SerializerOptions);

                // If deserialize result is null, create a new instance instead and save it.
                if (dataObject is null) {
                    dataObject = createFunc();
                    SaveFile(pluginInterface, filePath, dataObject);
                }
                
                return dataObject;
            }
            catch (Exception e) {
                // If there is any kind of error loading the file, generate a new one instead and save it.
                var localLog = pluginInterface.Create<LogWrapper>();
                localLog?.Log.Error($"Error trying to load file {filePath}, creating a new one instead.", e);
                
                SaveFile(pluginInterface, filePath, createFunc());
            }
        }

        var newFile = createFunc();
        SaveFile(pluginInterface, filePath, newFile);
        
        return newFile;
    }

    private static void SaveFile<T>(DalamudPluginInterface pluginInterface, string filePath, T file) {
        try {
            var fileText = JsonSerializer.Serialize(file, file!.GetType(), SerializerOptions);
            Dalamud.Utility.Util.WriteAllTextSafe(filePath, fileText);
        }
        catch (Exception e) {
            var localLog = pluginInterface.Create<LogWrapper>();
            localLog?.Log.Error($"Error trying to save file {filePath}", e);
        }
    } 

    internal static DirectoryInfo GetCharacterDirectoryInfo(this DalamudPluginInterface pluginInterface, ulong contentId) {
        var directoryInfo = new DirectoryInfo(Path.Combine(pluginInterface.ConfigDirectory.FullName, contentId.ToString()));
        
        if (directoryInfo is { Exists: false }) {
            directoryInfo.Create();
        }
        
        directoryInfo.Refresh();

        return directoryInfo;
    }

    internal static FileInfo GetCharacterFileInfo(this DalamudPluginInterface pluginInterface, ulong contentId, string fileName) 
        => new(Path.Combine(pluginInterface.GetCharacterDirectoryInfo(contentId).FullName, fileName));

    internal static IEnumerable<CharacterConfiguration> GetAllCharacterConfigurations(this DalamudPluginInterface pluginInterface) 
        => pluginInterface.GetAllCharacterContentIds().Select(pluginInterface.LoadCharacterConfiguration);

    private static CharacterConfiguration LoadCharacterConfiguration(this DalamudPluginInterface pluginInterface, ulong contentId) {
        var loadedConfiguration = pluginInterface.LoadCharacterFile(contentId, "System.config.json", () => CreateNewCharacterConfig(contentId));
        if (loadedConfiguration is { Version: not 2, ContentId: 0 }) {
            loadedConfiguration.Version = 2;
            loadedConfiguration.ContentId = contentId;
            pluginInterface.SaveCharacterFile(contentId, "System.config.json", loadedConfiguration);
        }

        return loadedConfiguration;
    }
    
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