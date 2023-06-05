using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class StringConfigOption : DrawableAttribute
{
    private readonly bool useAxisFont;
    
    public StringConfigOption(string labelLocKey, bool axisFont = false) : base(labelLocKey)
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
        }
        if (useAxisFont) ImGui.PopFont();

        if (ImGui.IsItemDeactivatedAfterEdit())
        {
            saveAction?.Invoke();
        }
    }
}