using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntCounterConfigOption : RightLabeledTabledDrawableAttribute
{
    private readonly string? helpTextKey;
    private string HelpText => TryGetLocalizedString(helpTextKey);

    public IntCounterConfigOption(string? label, string category, int group, string? helpText = null) : base(label, category, group)
    {
        helpTextKey = helpText;
    }
    
    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var intValue = GetValue<int>(obj, field);
        
        if (ImGui.InputInt($"##{field.Name}", ref intValue, 1))
        {
            SetValue(obj, field, intValue);
            saveAction?.Invoke();
        }
        
        if(helpTextKey is not null) ImGuiComponents.HelpMarker(HelpText);
    }
}