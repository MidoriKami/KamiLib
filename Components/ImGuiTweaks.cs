using System;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;
using KamiLib.Extensions;

namespace KamiLib.Components;

public static class ImGuiTweaks {
    public static bool ColorEditWithDefault(string label, ref Vector4 color, Vector4 defaultColor) {
        var valueChanged = ImGui.ColorEdit4($"##{label}", ref color, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);

        ImGui.SameLine();
        
        if (ImGui.Button($"Default##{label}")) {
            color = defaultColor;
        }

        ImGui.SameLine();

        ImGui.TextUnformatted(label);
        
        return valueChanged;
    }

    public static bool IconButtonWithSize(IFontHandle font, FontAwesomeIcon icon, string id, Vector2 size, string? tooltip = null) {
        using var imRaiiId = ImRaii.PushId(id);
        bool result;

        using (font.Push()) {
            result = ImGui.Button($"{icon.ToIconString()}", size);
        }

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled) && tooltip is not null) {
            ImGui.SetTooltip(tooltip);
        }

        return result;
    }

    public static void TextColoredUnformatted(Vector4 color, string text) {
        using var _ = ImRaii.PushColor(ImGuiCol.Text, color);
        ImGui.TextUnformatted(text);
    }

    public static bool EnumCombo<T>(string label, ref T refValue) where T : Enum {
        using var combo = ImRaii.Combo(label, refValue.GetDescription());
        if (!combo) return false;

        foreach (Enum enumValue in Enum.GetValues(refValue.GetType())) {
            if (ImGui.Selectable(enumValue.GetDescription(), enumValue.Equals(refValue))) {
                refValue = (T)enumValue;
                return true;
            }
        }

        return false;
    }
}