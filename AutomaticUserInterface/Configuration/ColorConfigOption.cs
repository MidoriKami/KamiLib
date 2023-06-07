using System;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface.Components;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.Graphics;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class ColorConfigOption : DrawableAttribute
{
    private readonly string? helpTextKey;

    public string HelpText => TryGetLocalizedString(helpTextKey);
    public Vector4 DefaultColor { get; init; }
    public Vector4 VectorByteColor => DefaultColor / 255;
    public ByteColor ByteColor => DefaultColor.ToByteColor();

    private string DefaultLabel => TryGetLocalizedString("Default");
    
    public ColorConfigOption(string label, string category, int group, float r, float g, float b, float a) : base(label, category, group)
    {
        DefaultColor = new Vector4(r, g, b, a);
    }

    public ColorConfigOption(string label, string category, int group, string helpText, float r, float g, float b, float a) : base(label, category, group)
    {
        DefaultColor = new Vector4(r, g, b, a);
        helpTextKey = helpText;
    }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null)
    {
        var vectorValue = GetValue<Vector4>(obj, field);
        
        if (ImGui.ColorEdit4($"##{field.Name}", ref vectorValue, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.AlphaBar))
        {
            SetValue(obj, field, vectorValue);
            saveAction?.Invoke();
        }
        
        ImGui.SameLine();
        
        if (ImGui.Button($"{DefaultLabel}##{field.Name}"))
        {
            SetValue(obj, field, DefaultColor);
            saveAction?.Invoke();
        }
        
        ImGui.SameLine();
        
        ImGui.Text(Label);
        
        if(helpTextKey is not null) ImGuiComponents.HelpMarker(HelpText);
    }
}