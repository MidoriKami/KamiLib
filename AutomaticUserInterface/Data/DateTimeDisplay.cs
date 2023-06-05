using System;
using System.Globalization;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class DateTimeDisplay : DrawableAttribute
{
    public DateTimeDisplay(string? labelLocKey) : base(labelLocKey) { }

    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null) => DrawTabled(obj, field);

    protected override void LeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
    }

    protected override void RightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var dateTime = GetValue<DateTime>(obj, field);
        
        ImGui.TextUnformatted(dateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture));
    }
}