using System;
using System.Reflection;
using Dalamud.Logging;

namespace KamiLib.AutomaticUserInterface;

public abstract class FieldAttributeBase : AttributeBase
{
    private readonly string? labelLocalizationKey;
    private readonly string categoryLocalizationKey;
    
    public int GroupIndex { get; init; }
    public string Label => TryGetLocalizedString(labelLocalizationKey);
    public string Category => TryGetLocalizedString(categoryLocalizationKey);

    protected bool HasLabel => labelLocalizationKey is not null;
    
    protected FieldAttributeBase(string? label, string categoryKey, int groupIndex)
    {
        labelLocalizationKey = label;
        categoryLocalizationKey = categoryKey;
        GroupIndex = groupIndex;
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