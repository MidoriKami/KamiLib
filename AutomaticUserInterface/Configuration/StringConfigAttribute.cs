using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class StringConfigAttribute : DrawableAttribute
{
    private readonly bool useAxisFont;
    
    public StringConfigAttribute(string label, bool axisFont = false) : base(label)
    {
        useAxisFont = axisFont;
    }

    protected override void Draw(object obj, MemberInfo field, Action? saveAction = null)
    {
        var stringValue = GetValue<string>(obj, field);

        if (useAxisFont) ImGui.PushFont(KamiCommon.FontManager.Axis12.ImFont);
        if (ImGui.InputTextWithHint($"##{field.Name}", Label, ref stringValue, 2048))
        {
            SetValue(obj, field, stringValue);
            saveAction?.Invoke();
        }
        if (useAxisFont) ImGui.PopFont();
    }
}