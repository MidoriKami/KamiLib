using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;

namespace KamiLib.AutomaticUserInterface;

public class EnumConfigOption : TabledDrawableAttribute
{
    private readonly string? helpTextKey;

    public string HelpText => TryGetLocalizedString(helpTextKey);
    
    public EnumConfigOption(string category, int group) : base(null, category, group) { }
    public EnumConfigOption(string label, string category, int group) : base(label, category, group) { }
    
    public EnumConfigOption(string label, string category, int group, string helpText) : base(label, category, group)
    {
        helpTextKey = helpText;
    }

    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var enumObject = GetValue<Enum>(obj, field);
        
        if (DrawEnumCombo(ref enumObject))
        {
            SetValue(obj, field, enumObject);
            saveAction?.Invoke();
        }
    }

    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
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