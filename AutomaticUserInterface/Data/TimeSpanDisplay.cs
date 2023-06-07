using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

/// <summary>
/// Displays time in the following format:
/// Day.Hour:Minute:Seconds
/// </summary>
public class TimeSpanDisplay : LeftLabeledTabledDrawableAttribute
{
    protected string TimeOutOfRangeString = "TimeNotAvailable";
    
    public TimeSpanDisplay(string? label, string category, int group) : base(label, category, group) { }
    
    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
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