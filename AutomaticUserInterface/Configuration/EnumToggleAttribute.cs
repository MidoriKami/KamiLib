using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;
using KamiLib.Utilities;

namespace KamiLib.AutomaticUserInterface.Configuration;

public class EnumToggleAttribute : DrawableAttribute
{
    public EnumToggleAttribute(string? label) : base(label) { }
    protected override void Draw(object obj, MemberInfo field, Action? saveAction = null)
    {
        var value = GetValue<Enum>(obj, field);
        
        ImGui.Columns(2);

        foreach (Enum enumValue in Enum.GetValues(value.GetType()))
        {
            var isFlagSet = value.HasFlag(enumValue);
            if(ImGuiComponents.ToggleButton(enumValue.ToString(), ref isFlagSet))
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
                    
                SetValue(obj, field, value);
                saveAction?.Invoke();
            }
            ImGui.SameLine();
            ImGui.TextUnformatted(enumValue.GetLabel());
            
            ImGui.NextColumn();
        }
        
        ImGui.Columns(1);
    }
}