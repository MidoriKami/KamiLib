using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class FloatConfigOption : RightLabeledTabledDrawableAttribute
{
    public float MinValue { get; init; }
    public float MaxValue { get; init; }

    public FloatConfigOption(string? label, string category, int group, float minValue = 0.0f, float maxValue = 1.0f) : base(label, category, group)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
    
    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var floatValue = GetValue<float>(obj, field);

        if (ImGui.DragFloat($"##DragFloat{Label}", ref floatValue, 0.01f * MaxValue, MinValue, MaxValue))
        {
            SetValue(obj, field, floatValue);
            saveAction?.Invoke();
        }
    }
}