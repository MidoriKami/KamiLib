using System;
using System.Numerics;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class Vector2ConfigAttribute : RightLabeledTabledDrawableAttribute
{
    public Vector2ConfigAttribute(string? label) : base(label) { }
    
    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var vectorValue = GetValue<Vector2>(obj, field);
        if (ImGui.DragFloat2($"##{field.Name}", ref vectorValue, 5.0f))
        {
            SetValue(obj, field, vectorValue);
            saveAction?.Invoke();
        }
    }
}