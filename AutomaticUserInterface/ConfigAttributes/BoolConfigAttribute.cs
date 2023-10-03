using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class BoolConfigAttribute : DrawableAttribute
{
    private readonly string? helpTextKey;
    private string HelpText => TryGetLocalizedString(helpTextKey);
    
    public BoolConfigAttribute(string label) : base(label) { }

    public BoolConfigAttribute(string label, string? helpText) : base(label)
    {
        helpTextKey = helpText;
    }
    
    protected override void Draw(object obj, MemberInfo field, Action? saveAction = null)
    {
        var value = GetValue<bool>(obj, field);

        if (ImGui.Checkbox(Label, ref value))
        {
            SetValue(obj, field, value);
            saveAction?.Invoke();
        }
        
        if(helpTextKey is not null) ImGuiComponents.HelpMarker(HelpText);
    }
}