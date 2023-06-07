using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntDisplay : LeftLabeledTabledDrawableAttribute
{
    public IntDisplay(string? label, string category, int group) : base(label, category, group) { }
    
    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var intValue = GetValue<int>(obj, field);
        
        ImGui.TextUnformatted(intValue.ToString());
    }
}