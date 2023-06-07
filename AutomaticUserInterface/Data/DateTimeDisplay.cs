using System;
using System.Globalization;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

/// <summary>
/// Displays the tagged date time as it was saved
/// </summary>
public class DateTimeDisplay : LeftLabeledTabledDrawableAttribute
{
    public DateTimeDisplay(string? label, string category, int group) : base(label, category, group) { }

    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var dateTime = GetValue<DateTime>(obj, field);
        
        ImGui.TextUnformatted(FormatDateTime(dateTime));
    }

    protected virtual string FormatDateTime(DateTime dateTime) => dateTime.ToString(CultureInfo.CurrentCulture);
}