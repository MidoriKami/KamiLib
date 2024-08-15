using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using ImGuiNET;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Classes;

public static class ImGuiTweaks {
    public static bool ColorEditWithDefault(string label, ref Vector4 color, Vector4 defaultColor) {
        var valueChanged = ImGui.ColorEdit4($"##{label}", ref color, ImGuiColorEditFlags.AlphaPreviewHalf | ImGuiColorEditFlags.NoInputs);

        ImGui.SameLine();
        
        if (ImGui.Button($"Default##{label}")) {
            color = defaultColor;
            valueChanged = true;
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
            if (!ImGui.Selectable(enumValue.GetDescription(), enumValue.Equals(refValue))) continue;
            
            refValue = (T)enumValue;
            return true;
        }

        return false;
    }

    public static bool Checkbox(string label, ref bool value, string? hintText) {
        using var group = ImRaii.Group();
        
        var result = ImGui.Checkbox(label, ref value);

        if (hintText is not null) {
            ImGuiComponents.HelpMarker(hintText);
        }

        return result;
    }

    public static bool PriorityInt(IDalamudPluginInterface pluginInterface, string label, ref int value) {
        ImGui.SetNextItemWidth(22.0f * ImGuiHelpers.GlobalScale);
        var valueChanged = ImGui.InputInt($"##{label}_input_int", ref value, 0, 0);
        
        using (pluginInterface.UiBuilder.IconFontFixedWidthHandle.Push()) {

            ImGui.SameLine();
            if (ImGui.Button(FontAwesomeIcon.ChevronUp.ToIconString())) {
                value++;
                valueChanged = true;
            }
            
            ImGui.SameLine();
            if (ImGui.Button(FontAwesomeIcon.ChevronDown.ToIconString())) {
                value--;
                valueChanged = true;
            }

            value = Math.Clamp(value, -9, 9);
        }
        
        ImGui.SameLine();
        ImGui.Text(label);
        
        return valueChanged;
    }

    public static bool UiColorPicker(string label, UIColor color) {
        var cursorStart = ImGui.GetCursorScreenPos();

        // Draw Rectangle
        ImGui.GetWindowDrawList().AddRectFilled(cursorStart, cursorStart + ImGuiHelpers.ScaledVector2(24.0f, 24.0f), ImGui.GetColorU32(color.Foreground()), 5.0f);
        ImGui.GetWindowDrawList().AddRect(cursorStart, cursorStart + ImGuiHelpers.ScaledVector2(24.0f, 24.0f), ImGui.GetColorU32(KnownColor.White.Vector() with { W = 0.33f }), 5.0f);

        // Draw dummy over rectangle
        ImGui.SetCursorScreenPos(cursorStart);
        ImGuiHelpers.ScaledDummy(24.0f, 24.0f);
        var result = ImGui.IsItemClicked();

        // Draw label
        ImGui.SameLine();
        ImGui.AlignTextToFramePadding();
        ImGui.Text(label);

        return result;
    }

    public static void TableRow(string leftColumn, string rightColumn) {
        ImGui.TableNextColumn();
        ImGui.Text(leftColumn);

        ImGui.TableNextColumn();
        ImGui.Text(rightColumn);
    }

    public static bool SliderUint(string label, ref uint value, uint minValue, uint maxValue) {
        var intValue = (int) value;

        var result = ImGui.SliderInt(label, ref intValue, (int) minValue, (int) maxValue);

        if (result) {
            value = (uint) intValue;
        }

        return result;
    }

    public static void Header(string label, bool center = false) {
        ImGuiHelpers.ScaledDummy(10.0f);
        if (center) {
            ImGuiHelpers.CenteredText(label);
        }
        else {
            ImGui.Text(label);
        }
        ImGui.Separator();
        ImGuiHelpers.ScaledDummy(5.0f);
    }
}