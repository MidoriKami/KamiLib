using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;

namespace KamiLib.AutomaticUserInterface;

public class EnumConfigOption : DrawableAttribute
{
    private readonly string? helpTextKey;

    public string HelpText => TryGetLocalizedString(helpTextKey);
    
    public EnumConfigOption() : base(null)
    {
        
    }

    public EnumConfigOption(string label) : base(label)
    {
        
    }
    
    public EnumConfigOption(string label, string helpText) : base(label)
    {
        helpTextKey = helpText;
    }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null) => DrawTabled(obj, field, saveAction);

    protected override void LeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var enumObject = GetValue<Enum>(obj, field);
        
        if (DrawEnumCombo(ref enumObject))
        {
            SetValue(obj, field, enumObject);
            saveAction?.Invoke();
        }
    }

    protected override void RightColumn(object obj, FieldInfo field, Action? saveAction = null)
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