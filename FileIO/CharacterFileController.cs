using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Newtonsoft.Json;

namespace KamiLib.FileIO;

public static unsafe class CharacterFileController
{
    public static T LoadFile<T>(string filePath, object targetObject)
    {
        Service.Log.Verbose($"[FileController] Loading {filePath}");

        if (LoadFile(filePath, targetObject.GetType(), out var loadedData))
        {
            return (T) loadedData;
        }

        Service.Log.Verbose($"[FileController] File Doesn't Exist, creating: {filePath}");

        SaveFile(filePath, targetObject.GetType(), targetObject);
        return (T) targetObject;
    }

    private static bool LoadFile(string fileName, Type fileType, [NotNullWhen(true)] out object? loadedData)
    {
        try
        {
            var fileInfo = GetFileInfo(fileName);

            if (fileInfo is { Exists: false })
            {
                loadedData = null;
                return false;
            }

            var jsonString = File.ReadAllText(fileInfo.FullName);
            loadedData = JsonConvert.DeserializeObject(jsonString, fileType, new JsonSerializerSettings
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace
            })!;
            return true;
        }
        catch (Exception exception)
        {
            Service.Log.Error(exception, $"[FileController] Failed to load file: {fileName}");

            loadedData = null;
            return false;
        }
    }

    public static void SaveFile(string fileName, Type fileType, object objectData)
    {
        if (Service.ClientState.LocalContentId is 0) return;

        Service.Log.Verbose($"[FileController] Saving {fileName}");

        try
        {
            var fileInfo = GetFileInfo(fileName);

            var jsonString = JsonConvert.SerializeObject(objectData, fileType, new JsonSerializerSettings { Formatting = Formatting.Indented });
            Util.WriteAllTextSafe(fileInfo.FullName, jsonString);
        }
        catch (Exception exception)
        {
            Service.Log.Error(exception, $"[FileController] Failed to save file: {fileName}");
        }
    }

    private static FileInfo GetFileInfo(string fileName)
    {
        var contentId = PlayerState.Instance()->ContentId;
        var configDirectory = GetCharacterDirectory(contentId);

        return new FileInfo(Path.Combine(configDirectory.FullName, fileName));
    }

    private static DirectoryInfo GetCharacterDirectory(ulong contentId)
    {
        var directoryInfo = new DirectoryInfo(Path.Combine(Service.PluginInterface.ConfigDirectory.FullName, contentId.ToString()));

        if (directoryInfo is { Exists: false } && Service.ClientState.LocalContentId is not 0)
        {
            directoryInfo.Create();
        }

        return directoryInfo;
    }
}