using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;

namespace KamiLib.AutomaticUserInterface.Configuration;

public class RadioEnumConfigAttribute : RightLabeledTabledDrawableAttribute
{
    private readonly string? helpTextKey;

    public string HelpText => TryGetLocalizedString(helpTextKey);
    
    public RadioEnumConfigAttribute(string? label) : base(label)
    {
        
    }
    
    public RadioEnumConfigAttribute(string label, string helpText) : base(label)
    {
        helpTextKey = helpText;
    }

    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        base.DrawRightColumn(obj, field, saveAction);
        if (helpTextKey is not null) ImGuiComponents.HelpMarker(HelpText);
    }

    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var enumObject = GetValue<Enum>(obj, field);
        var firstLine = true;
        
        foreach (Enum enumValue in Enum.GetValues(enumObject.GetType()))
        {
            if (!firstLine) ImGui.SameLine();
            
            if (ImGui.RadioButton(enumValue.GetLabel(), enumValue.Equals(enumObject)))
            {
                SetValue(obj, field, enumValue);
                saveAction?.Invoke();
            }

            firstLine = false;
        }
    }
}