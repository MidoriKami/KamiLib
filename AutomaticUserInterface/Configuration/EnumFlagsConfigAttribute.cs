using System;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.Localization;
using KamiLib.Utilities;

namespace KamiLib.AutomaticUserInterface;

public class EnumFlagsConfigAttribute : EnumConfigAttribute
{
    public EnumFlagsConfigAttribute() { }
    public EnumFlagsConfigAttribute(string label) : base(label) { }

    public EnumFlagsConfigAttribute(string label, string helpText) : base(label, helpText) { }
    
    protected override bool DrawEnumCombo(ref Enum value)
    {
        var valueChanged = false;
        
        if (ImGui.BeginCombo($"##EnumCombo{value.GetType()}", Strings.SelectMultiple))
        {
            foreach (Enum enumValue in Enum.GetValues(value.GetType()))
            {
                if (value.HasFlag(enumValue))
                {
                    ImGui.PushFont(UiBuilder.IconFont);
                    ImGui.TextUnformatted(FontAwesomeIcon.Check.ToIconString());
                    ImGui.PopFont();
                }
                else
                {
                    ImGuiHelpers.ScaledDummy(16.0f);
                }
                
                ImGui.SameLine();
                
                if (ImGui.Selectable(enumValue.GetLabel(), false))
                {
                    var sourceValue = Convert.ToInt32(value);
                    var targetValue = Convert.ToInt32(enumValue);

                    if (value.HasFlag(enumValue))
                    {
                        value = (Enum) Enum.ToObject(value.GetType(), sourceValue & ~targetValue);
                    }
                    else
                    {
                        value = (Enum) Enum.ToObject(value.GetType(), sourceValue | targetValue);
                    }
                    
                    valueChanged = true;
                }
            }
            
            ImGui.EndCombo();
        }

        return valueChanged;
    }
}