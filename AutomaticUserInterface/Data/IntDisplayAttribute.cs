using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntDisplayAttribute : LeftLabeledTabledDrawableAttribute
{
    public IntDisplayAttribute(string? label) : base(label) { }
    
    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var intValue = GetValue<int>(obj, field);
        
        ImGui.TextUnformatted(intValue.ToString());
    }
}