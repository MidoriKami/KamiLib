using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Dalamud.Plugin;
using Dalamud.Utility;
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
    /// <typeparam name="T">Type of file to load, needed for serialization.</typeparam>
    /// <returns>Either a loaded config file, or a new'd file of type T.</returns>
    public static T LoadCharacterFile<T>(this IDalamudPluginInterface pluginInterface, ulong contentId, string fileName) where T : new() 
        => LoadFile<T>(pluginInterface, pluginInterface.GetCharacterFileInfo(contentId, fileName).FullName);

    /// <summary>
    /// Load a configuration file from the plugin specific config directory.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface used to find the correct directory.</param>
    /// <param name="fileName">Specific name of the file you wish you load.</param>
    /// <typeparam name="T">Type of file to load, needed for serialization.</typeparam>
    /// <returns>Either a loaded config file, or a new'd file of type T.</returns>
    public static T LoadConfigFile<T>(this IDalamudPluginInterface pluginInterface, string fileName) where T : new() 
        => LoadFile<T>(pluginInterface, Path.Combine(pluginInterface.ConfigDirectory.FullName, fileName));

    /// <summary>
    /// Save a configuration file for a specific character.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface used to find the correct directory.</param>
    /// <param name="contentId">ContentId of the desired character file.</param>
    /// <param name="fileName">Specific name of the file you wish to save.</param>
    /// <param name="file">The object to write to a file.</param>
    /// <typeparam name="T">Type of file to load, needed for serialization.</typeparam>
    public static void SaveCharacterFile<T>(this IDalamudPluginInterface pluginInterface, ulong contentId, string fileName, T file) 
        => SaveFile(pluginInterface, pluginInterface.GetCharacterFileInfo(contentId, fileName).FullName, file);

    /// <summary>
    /// Save a configuration file to the plugin specific config directory.
    /// </summary>
    /// <param name="pluginInterface">Dalamud Plugin Interface used to find the correct directory.</param>
    /// <param name="fileName">Specific name of the file you wish to save.</param>
    /// <param name="file">The object to write to a file.</param>
    /// <typeparam name="T">Type of file to load, needed for serialization.</typeparam>
    public static void SaveConfigFile<T>(this IDalamudPluginInterface pluginInterface, string fileName, T file)
        => SaveFile(pluginInterface, Path.Combine(pluginInterface.GetPluginConfigDirectory(), fileName), file);

    private static T LoadFile<T>(IDalamudPluginInterface pluginInterface, string filePath) where T : new() {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo is { Exists: true }) {
            try {
                var fileText = File.ReadAllText(fileInfo.FullName);
                var dataObject = JsonSerializer.Deserialize<T>(fileText, SerializerOptions);

                // If deserialize result is null, create a new instance instead and save it.
                if (dataObject is null) {
                    dataObject = new T();
                    SaveFile(pluginInterface, filePath, dataObject);
                }
                
                return dataObject;
            }
            catch (Exception e) {
                // If there is any kind of error loading the file, generate a new one instead and save it.
                var localLog = pluginInterface.Create<LogWrapper>();
                localLog?.Log.Error(e, $"Error trying to load file {filePath}, creating a new one instead.");
                
                SaveFile(pluginInterface, filePath, new T());
            }
        }

        var newFile = new T();
        SaveFile(pluginInterface, filePath, newFile);
        
        return newFile;
    }

    private static void SaveFile<T>(IDalamudPluginInterface pluginInterface, string filePath, T file) {
        try {
            var fileText = JsonSerializer.Serialize(file, file!.GetType(), SerializerOptions);
            FilesystemUtil.WriteAllTextSafe(filePath, fileText);
        }
        catch (Exception e) {
            var localLog = pluginInterface.Create<LogWrapper>();
            localLog?.Log.Error(e, $"Error trying to save file {filePath}");
        }
    } 
 
    internal static DirectoryInfo GetCharacterDirectoryInfo(this IDalamudPluginInterface pluginInterface, ulong contentId) {
        var directoryInfo = new DirectoryInfo(Path.Combine(pluginInterface.ConfigDirectory.FullName, contentId.ToString()));
        
        if (directoryInfo is { Exists: false }) {
            directoryInfo.Create();
        }
        
        directoryInfo.Refresh();

        return directoryInfo;
    }

    public static FileInfo GetCharacterFileInfo(this IDalamudPluginInterface pluginInterface, ulong contentId, string fileName) 
        => new(Path.Combine(pluginInterface.GetCharacterDirectoryInfo(contentId).FullName, fileName));

    internal static IEnumerable<CharacterConfiguration> GetAllCharacterConfigurations(this IDalamudPluginInterface pluginInterface) 
        => pluginInterface.GetAllCharacterContentIds().Select(pluginInterface.LoadCharacterConfiguration);

    private static CharacterConfiguration LoadCharacterConfiguration(this IDalamudPluginInterface pluginInterface, ulong contentId) {
        var loadedConfiguration = pluginInterface.LoadCharacterFile<CharacterConfiguration>(contentId, "System.config.json");

        if (loadedConfiguration.ContentId is 0) {
            loadedConfiguration.ContentId = contentId;
        }
        
        if (loadedConfiguration is { Version: not 2, ContentId: 0 }) {
            loadedConfiguration.Version = 2;
            loadedConfiguration.ContentId = contentId;
            pluginInterface.SaveCharacterFile(contentId, "System.config.json", loadedConfiguration);
        }

        return loadedConfiguration;
    }
    
    private static IEnumerable<ulong> GetAllCharacterContentIds(this IDalamudPluginInterface pluginInterface) {
        if (pluginInterface.ConfigDirectory is { Exists: true } directoryInfo) {
            foreach (var directory in directoryInfo.EnumerateDirectories()) {
                if (ulong.TryParse(directory.Name, out var contentId)) {
                    yield return contentId;
                }
            }
        }
    }
}
