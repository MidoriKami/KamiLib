using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntCounterConfigAttribute : RightLabeledTabledDrawableAttribute
{
    private readonly string? helpTextKey;
    private string HelpText => TryGetLocalizedString(helpTextKey);

    private readonly bool showStep;

    public IntCounterConfigAttribute(string? label, string? helpText = null) : base(label)
    {
        helpTextKey = helpText;
        showStep = true;
    }
    
    public IntCounterConfigAttribute(string? label, bool showStepButtons, string? helpText = null) : base(label)
    {
        helpTextKey = helpText;
        showStep = showStepButtons;
    }
    
    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
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