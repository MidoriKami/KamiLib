using System;
using System.Numerics;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class PositionConfigOption : DrawableAttribute
{

    public PositionConfigOption(string? labelLocKey) : base(labelLocKey)
    {
        
    }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null) => DrawTabled(obj, field, saveAction);

    protected override void LeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var vectorValue = GetValue<Vector2>(obj, field);
        if (ImGui.DragFloat2($"##{field.Name}", ref vectorValue, 5.0f))
        {
            SetValue(obj, field, vectorValue);
            saveAction?.Invoke();
        }
    }
}