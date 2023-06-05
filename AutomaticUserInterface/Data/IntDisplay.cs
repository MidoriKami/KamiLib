using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntDisplay : DrawableAttribute
{
    public IntDisplay(string? labelLocKey) : base(labelLocKey)
    {
        
    }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null) => DrawTabled(obj, field);

    protected override void LeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.Text(Label);
    }

    protected override void RightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var intValue = GetValue<int>(obj, field);
        
        ImGui.TextUnformatted(intValue.ToString());
    }
}