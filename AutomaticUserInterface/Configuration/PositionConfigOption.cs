using System;
using System.Numerics;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class PositionConfigOption : RightLabeledTabledDrawableAttribute
{
    public PositionConfigOption(string? label, string category, int group) : base(label, category, group) { }
    
    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var vectorValue = GetValue<Vector2>(obj, field);
        if (ImGui.DragFloat2($"##{field.Name}", ref vectorValue, 5.0f))
        {
            SetValue(obj, field, vectorValue);
            saveAction?.Invoke();
        }
    }
}