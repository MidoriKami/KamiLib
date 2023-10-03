using System;
using System.Drawing;
using System.Numerics;
using System.Reflection;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class ColorConfigAttribute : DrawableAttribute
{
    private readonly string? helpTextKey;

    public string HelpText => TryGetLocalizedString(helpTextKey);
    public Vector4? DefaultColor { get; init; }

    private string DefaultLabel => TryGetLocalizedString("Default");

    public ColorConfigAttribute(string label) : base(label) { }
    
    public ColorConfigAttribute(string label, float r, float g, float b, float a) : base(label)
    {
        DefaultColor = new Vector4(r, g, b, a);
    }

    public ColorConfigAttribute(string label, string helpText, float r, float g, float b, float a) : base(label)
    {
        DefaultColor = new Vector4(r, g, b, a);
        helpTextKey = helpText;
    }

    public ColorConfigAttribute(string label, byte r, byte g, byte b, byte a) : base(label)
    {
        DefaultColor = new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
    }

    public ColorConfigAttribute(string label, string hexColor) : base(label)
    {
        DefaultColor = ColorTranslator.FromHtml(hexColor).ToKnownColor().Vector();
    }
    
    protected override void Draw(object obj, MemberInfo field, Action? saveAction = null)
    {
        var vectorValue = GetValue<Vector4>(obj, field);
        
        if (ImGui.ColorEdit4($"##{field.Name}", ref vectorValue, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.AlphaBar))
        {
            SetValue(obj, field, vectorValue);
            saveAction?.Invoke();
        }

        if (DefaultColor is not null)
        {
            ImGui.SameLine();
        
            if (ImGui.Button($"{DefaultLabel}##{field.Name}"))
            {
                SetValue(obj, field, DefaultColor);
                saveAction?.Invoke();
            }
        }
        
        ImGui.SameLine();
        ImGui.Text(Label);
        
        if(helpTextKey is not null) ImGuiComponents.HelpMarker(HelpText);
    }
}