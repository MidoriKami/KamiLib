using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class StringConfigOption : DrawableAttribute
{
    private readonly bool useAxisFont;
    
    public StringConfigOption(string label, string category, int group, bool axisFont = false) : base(label, category, group)
    {
        useAxisFont = axisFont;
    }

    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null)
    {
        var stringValue = GetValue<string>(obj, field);

        if (useAxisFont) ImGui.PushFont(KamiCommon.FontManager.Axis12.ImFont);
        if (ImGui.InputTextWithHint($"##{field.Name}", Label, ref stringValue, 2048))
        {
            field.SetValue(obj, stringValue);
            saveAction?.Invoke();
        }
        if (useAxisFont) ImGui.PopFont();
    }
}