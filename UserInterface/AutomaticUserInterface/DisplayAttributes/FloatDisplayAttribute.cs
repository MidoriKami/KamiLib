using System;
using System.Globalization;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class FloatDisplayAttribute : LeftLabeledTabledDrawableAttribute
{
    public FloatDisplayAttribute(string? label) : base(label)
    {
    }

    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var floatValue = GetValue<float>(obj, field);

        ImGui.TextUnformatted(floatValue.ToString(CultureInfo.CurrentCulture));
    }
}