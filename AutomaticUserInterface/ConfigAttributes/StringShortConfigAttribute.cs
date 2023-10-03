using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class StringShortConfigAttribute : TabledDrawableAttribute
{
    public StringShortConfigAttribute(string label) : base(label)
    {

    }
    
    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var stringValue = GetValue<string>(obj, field);

        if (ImGui.InputTextWithHint($"##{field.Name}", Label, ref stringValue, 2048))
        {
            SetValue(obj, field, stringValue);
            saveAction?.Invoke();
        }
    }
    
    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        // This side empty intentionally
    }
}