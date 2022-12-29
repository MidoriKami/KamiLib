using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Newtonsoft.Json.Linq;

namespace KamiLib.Configuration;

public static class Migrate
{
    private static JObject? _parsedJson;
    
    public static void LoadFile(FileInfo configFilePath)
    {
        var reader = new StreamReader(configFilePath.FullName);
        var fileText = reader.ReadToEnd();
        reader.Dispose();
        
        _parsedJson = JObject.Parse(fileText);
    }

    public static int GetFileVersion()
    {
        return _parsedJson?.GetValue("Version")?.Value<int>() ?? 0;
    }
    
    public static Setting<T> GetSettingValue<T>(string key) where T : struct
    {
        return new Setting<T>(_parsedJson!.SelectToken(key)!.Value<T>());
    }

    public static Setting<T> GetSettingEnum<T>(string key) where T : struct
    {
        var readValue = _parsedJson!.SelectToken(key)!.Value<int>();

        return new Setting<T>((T) Enum.ToObject(typeof(T), readValue));
    }

    public static T GetValue<T>(string key)
    {
        return _parsedJson!.SelectToken(key)!.Value<T>()!;
    }

    public static JArray GetArray(string key)
    {
        return (JArray) _parsedJson!.SelectToken(key)!;
    }

    public static List<T> GetArray<T>(string key)
    {
        var array = GetArray(key);

        return array.ToObject<List<T>>()!;
    }
    
    public static Setting<Vector4> GetVector4(string key)
    {
        return new Setting<Vector4>(new Vector4
        {
            X = GetValue<float>($"{key}.X"),
            Y = GetValue<float>($"{key}.Y"),
            Z = GetValue<float>($"{key}.Z"),
            W = GetValue<float>($"{key}.W"),
        });
    }
}