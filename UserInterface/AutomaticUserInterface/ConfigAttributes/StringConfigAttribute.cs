using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class StringConfigAttribute : DrawableAttribute
{
    public StringConfigAttribute(string label) : base(label)
    {
    }

    protected override void Draw(object obj, MemberInfo field, Action? saveAction = null)
    {
        var stringValue = GetValue<string>(obj, field);

        if (ImGui.InputTextWithHint($"##{field.Name}", Label, ref stringValue, 2048))
        {
            SetValue(obj, field, stringValue);
            saveAction?.Invoke();
        }
    }
}