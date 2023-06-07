using System;
using System.Globalization;

namespace KamiLib.AutomaticUserInterface;

/// <summary>
/// Displays DateTime field in LocalTime
/// </summary>
public class LocalDateTimeDisplay : DateTimeDisplay
{
    public LocalDateTimeDisplay(string? label, string category, int group) : base(label, category, group) { }
    
    protected override string FormatDateTime(DateTime dateTime) => dateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
}