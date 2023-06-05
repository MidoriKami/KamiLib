using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class BoolDescriptionConfigOption : BoolConfigOption
{
    private readonly string? descriptionLocKey;

    public string Description => TryGetLocalizedString(descriptionLocKey);
    
    public BoolDescriptionConfigOption(string labelLocKey, string description) : base(labelLocKey, null)
    {
        descriptionLocKey = description;
    }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.Text(Description);
        
        base.Draw(obj, field, saveAction);
    }
}