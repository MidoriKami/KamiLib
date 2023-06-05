using System;
using System.Globalization;

namespace KamiLib.AutomaticUserInterface;

/// <summary>
/// Displays DateTime field in LocalTime
/// </summary>
public class LocalDateTimeDisplay : DateTimeDisplay
{
    public LocalDateTimeDisplay(string? labelLocKey) : base(labelLocKey) { }
    
    protected override string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
    }
}