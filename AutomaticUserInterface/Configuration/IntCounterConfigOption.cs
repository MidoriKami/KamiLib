using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntCounterConfigOption : RightLabeledTabledDrawableAttribute
{
    private readonly string? helpTextKey;
    private string HelpText => TryGetLocalizedString(helpTextKey);

    private readonly bool showStep;

    public IntCounterConfigOption(string? label, string category, int group, string? helpText = null) : base(label, category, group)
    {
        helpTextKey = helpText;
        showStep = true;
    }
    
    public IntCounterConfigOption(string? label, string category, int group, bool showStepButtons, string? helpText = null) : base(label, category, group)
    {
        helpTextKey = helpText;
        showStep = showStepButtons;
    }
    
    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var intValue = GetValue<int>(obj, field);
        
        if (ImGui.InputInt($"##{field.Name}", ref intValue, showStep ? 1 : 0))
        {
            SetValue(obj, field, intValue);
            saveAction?.Invoke();
        }
        
        if(helpTextKey is not null) ImGuiComponents.HelpMarker(HelpText);
    }
}