using System;
using System.Linq;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntComboConfigOption : IntConfigOption
{
    private readonly string? helpTextKey;
    private string HelpText => TryGetLocalizedString(helpTextKey);

    public IntComboConfigOption(string label, string category, int group, int minValue, int maxValue, string? helpText = null) : base(label, category, group, minValue, maxValue)
    {
        helpTextKey = helpText;
    }

    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var intValue = GetValue<int>(obj, field);
        var range = MaxValue - MinValue;

        if (ImGui.BeginCombo($"##{field.Name}", intValue.ToString()))
        {
            foreach (var value in Enumerable.Range(MinValue, range + 1))
            {
                if (ImGui.Selectable(value.ToString(), intValue == value))
                {
                    SetValue(obj, field, value);
                    saveAction?.Invoke();
                }
            }
            
            ImGui.EndCombo();
        }
        
    }

    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
        
        if(helpTextKey is not null) ImGuiComponents.HelpMarker(HelpText);
    }
}