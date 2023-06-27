using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class StringShortConfigAttribute : TabledDrawableAttribute
{
    private readonly bool useAxisFont;
    
    public StringShortConfigAttribute(string label, bool axisFont = false) : base(label)
    {
        useAxisFont = axisFont;
    }
    
    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
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
    
    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        // This side empty intentionally
    }
}