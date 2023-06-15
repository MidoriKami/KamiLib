using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntCounterConfigOption : RightLabeledTabledDrawableAttribute
{
    public IntCounterConfigOption(string? label, string category, int group) : base(label, category, group) { }
    
    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var intValue = GetValue<int>(obj, field);
        
        if (ImGui.InputInt($"##{field.Name}", ref intValue, 1))
        {
            SetValue(obj, field, intValue);
            saveAction?.Invoke();
        }
    }
}