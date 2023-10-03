using System;
using System.Globalization;

namespace KamiLib.AutomaticUserInterface;

/// <summary>
///     Displays DateTime field in LocalTime
/// </summary>
public class LocalDateTimeDisplayAttribute : DateTimeDisplayAttribute
{
    public LocalDateTimeDisplayAttribute(string? label) : base(label)
    {
    }

    protected override string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToLocalTime().ToString(CultureInfo.CurrentCulture);
    }
}