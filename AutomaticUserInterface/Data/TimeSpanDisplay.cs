using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class TimeSpanDisplay : DrawableAttribute
{
    protected string TimeOutOfRangeString = "TimeNotAvailable";
    
    public TimeSpanDisplay(string? labelLocKey) : base(labelLocKey) { }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null) => DrawTabled(obj, field);

    protected override void LeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
    }

    protected override void RightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var timeSpan = GetValue<TimeSpan>(obj, field);
        
        if (timeSpan > TimeSpan.MinValue)
        {
            ImGui.Text($"{timeSpan.Days:0}.{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}");
        }
        else
        {
            ImGui.TextUnformatted(TryGetLocalizedString(TimeOutOfRangeString));
        }
    }
}