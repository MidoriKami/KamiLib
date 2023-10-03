using System;
using System.Numerics;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class Vector2ConfigAttribute : RightLabeledTabledDrawableAttribute
{
    private Vector2? Minimum { get; }
    private Vector2? Maximum { get; }
    private float Speed { get; }
    
    public Vector2ConfigAttribute(string? label, float speed = 5.0f) : base(label)
    {
        Speed = speed;
    }

    public Vector2ConfigAttribute(string? label, float minX, float minY, float maxX, float maxY, float speed = 5.0f) : base(label)
    {
        Minimum = new Vector2(minX, minY);
        Maximum = new Vector2(maxX, maxY);
        Speed = speed;
    }

    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var vectorValue = GetValue<Vector2>(obj, field);
        if (ImGui.DragFloat2($"##{field.Name}", ref vectorValue, Speed))
        {
            if (Minimum is not null && Maximum is not null)
            {
                var clampedX = Math.Clamp(vectorValue.X, Minimum.Value.X, Maximum.Value.X);
                var clampedY = Math.Clamp(vectorValue.Y, Minimum.Value.Y, Maximum.Value.Y);
                vectorValue = new Vector2(clampedX, clampedY);
            }

            SetValue(obj, field, vectorValue);
            saveAction?.Invoke();
        }
    }
}