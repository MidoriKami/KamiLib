using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class ShortStringConfigOption : TabledDrawableAttribute
{
    private readonly bool useAxisFont;
    
    public ShortStringConfigOption(string label, string category, int group, bool axisFont = false) : base(label, category, group)
    {
        useAxisFont = axisFont;
    }
    
    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
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
    
    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        // This side empty intentionally
    }
}