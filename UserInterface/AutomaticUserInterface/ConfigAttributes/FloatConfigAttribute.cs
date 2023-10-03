using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class FloatConfigAttribute : RightLabeledTabledDrawableAttribute
{
    public float MinValue { get; init; }
    public float MaxValue { get; init; }
    
    public FloatConfigAttribute(string? label, float minValue = 0.0f, float maxValue = 1.0f) : base(label)
    {
        MinValue = minValue;
        MaxValue = maxValue;
    }
    
    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var floatValue = GetValue<float>(obj, field);

        if (ImGui.DragFloat($"##DragFloat{Label}", ref floatValue, 0.01f * MaxValue, MinValue, MaxValue))
        {
            SetValue(obj, field, floatValue);
            saveAction?.Invoke();
        }
    }
}