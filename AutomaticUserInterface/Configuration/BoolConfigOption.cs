using System;
using System.Reflection;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class BoolConfigOption : DrawableAttribute
{
    private readonly string? helpTextKey;
    private string HelpText => TryGetLocalizedString(helpTextKey);
    
    public BoolConfigOption(string label, string category, int group) : base(label, category, group) { }

    public BoolConfigOption(string label, string category, int group, string? helpText) : base(label, category, group)
    {
        helpTextKey = helpText;
    }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null)
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