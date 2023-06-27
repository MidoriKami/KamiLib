using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

/// <summary>
/// Displays time in the following format:
/// Day.Hour:Minute:Seconds
/// </summary>
public class TimeSpanDisplayAttribute : LeftLabeledTabledDrawableAttribute
{
    protected string TimeOutOfRangeString = "TimeNotAvailable";
    
    public TimeSpanDisplayAttribute(string? label) : base(label) { }
    
    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var timeSpan = GetValue<TimeSpan>(obj, field);
        
        if (timeSpan > TimeSpan.MinValue)
        {
            ImGui.TextUnformatted(FormatTime(timeSpan));
        }
        else
        {
            ImGui.TextUnformatted(TryGetLocalizedString(TimeOutOfRangeString));
        }
    }
    
    protected virtual string FormatTime(TimeSpan timeSpan)
    {
        return $"{timeSpan.Days:0}.{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
    }
}