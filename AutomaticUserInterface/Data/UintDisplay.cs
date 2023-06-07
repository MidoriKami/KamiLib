using System;
using System.Globalization;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class UintDisplay : LeftLabeledTabledDrawableAttribute
{
    public UintDisplay(string? label, string category, int group) : base(label, category, group) { }

    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var uintValue = GetValue<uint>(obj, field);
        
        ImGui.TextUnformatted(uintValue.ToString(CultureInfo.CurrentCulture));
    }
}