using System;
using System.Reflection;
using Dalamud.Logging;

namespace KamiLib.AutomaticUserInterface;

public abstract class AttributeBase : Attribute
{
    private readonly string? labelLocalizationKey;
    private readonly string categoryLocalizationKey;
    public int GroupIndex { get; init; } 

    public string Label => TryGetLocalizedString(labelLocalizationKey);
    public string Category => TryGetLocalizedString(categoryLocalizationKey);

    protected bool HasLabel => labelLocalizationKey is not null;
    
    protected AttributeBase(string? label, string categoryKey, int groupIndex)
    {
        labelLocalizationKey = label;
        categoryLocalizationKey = categoryKey;
        GroupIndex = groupIndex;
    }
    
    protected static string TryGetLocalizedString(string? key)
    {
        if (key is not null && KamiCommon.Localization is { } wrapper)
        {
            var resolvedString = wrapper.GetTranslatedString(key);
            
            return string.IsNullOrEmpty(resolvedString) ? $"[[{key}]]" : resolvedString;
        }

        return string.IsNullOrEmpty(key) ? "[[invalid key]]" : key;
    }
    
    protected T GetValue<T>(object obj, FieldInfo fieldInfo)
    {
        try
        {
            var value = fieldInfo.GetValue(obj)!;

            return (T) value;
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Reflection Error");
            throw;
        }
    }

    protected void SetValue<T>(object obj, FieldInfo fieldInfo, T value)
    {
        fieldInfo.SetValue(obj, value);
    }
}