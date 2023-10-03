using System;
using System.Globalization;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class UintDisplayAttribute : LeftLabeledTabledDrawableAttribute
{
    public UintDisplayAttribute(string? label) : base(label) { }

    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var uintValue = GetValue<uint>(obj, field);
        
        ImGui.TextUnformatted(uintValue.ToString(CultureInfo.CurrentCulture));
    }
}