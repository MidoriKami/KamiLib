using System.Numerics;
using ImGuiNET;

namespace KamiLib.TabBar;

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
}