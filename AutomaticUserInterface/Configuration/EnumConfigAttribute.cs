using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;

namespace KamiLib.AutomaticUserInterface;

public class EnumConfigAttribute : TabledDrawableAttribute
{
    private readonly string? helpTextKey;

    public string HelpText => TryGetLocalizedString(helpTextKey);
    
    public EnumConfigAttribute() : base(null) { }
    public EnumConfigAttribute(string label) : base(label) { }
    
    public EnumConfigAttribute(string label, string helpText) : base(label)
    {
        helpTextKey = helpText;
    }

    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        var enumObject = GetValue<Enum>(obj, field);
        
        if (DrawEnumCombo(ref enumObject))
        {
            SetValue(obj, field, enumObject);
            saveAction?.Invoke();
        }
    }

    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        if(HasLabel) ImGui.TextUnformatted(Label);
        if(helpTextKey is not null) ImGuiComponents.HelpMarker(HelpText);
    }

    protected bool DrawEnumCombo(ref Enum value)
    {
        var valueChanged = false;
        
        if (ImGui.BeginCombo($"##EnumCombo{value.GetType()}", value.GetLabel()))
        {
            foreach (Enum enumValue in Enum.GetValues(value.GetType()))
            {
                if (ImGui.Selectable(enumValue.GetLabel(), enumValue.Equals(value)))
                {
                    value = enumValue;
                    valueChanged = true;
                }
            }
            
            ImGui.EndCombo();
        }

        return valueChanged;
    }
}