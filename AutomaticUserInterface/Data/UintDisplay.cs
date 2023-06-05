using System;
using System.Globalization;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class UintDisplay : LeftLabeledTabledDrawableAttribute
{
    public UintDisplay(string? labelLocKey) : base(labelLocKey) { }

    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var uintValue = GetValue<uint>(obj, field);
        
        ImGui.TextUnformatted(uintValue.ToString(CultureInfo.CurrentCulture));
    }
}