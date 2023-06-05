using System;
using System.Globalization;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class FloatDisplay : LeftLabeledTabledDrawableAttribute
{
    public FloatDisplay(string? labelLocKey) : base(labelLocKey) { }
    
    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var floatValue = GetValue<float>(obj, field);
        
        ImGui.TextUnformatted(floatValue.ToString(CultureInfo.CurrentCulture));
    }
}