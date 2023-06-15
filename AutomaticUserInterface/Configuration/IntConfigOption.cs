using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

/// <summary>
/// Int Slider Configuration Options
/// </summary>
public class IntConfigOption : RightLabeledTabledDrawableAttribute
{
    public int MinValue { get; init; }
    public int MaxValue { get; init; }

    public IntConfigOption(string label, string category, int group, int minValue, int maxValue) : base(label, category, group)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
    
    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var intValue = GetValue<int>(obj, field);
        
        if (ImGui.SliderInt($"##{field.Name}", ref intValue, MinValue, MaxValue))
        {
            SetValue(obj, field, intValue);
            saveAction?.Invoke();
        }
    }
}