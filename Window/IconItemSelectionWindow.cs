using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace KamiLib.Window;

public abstract class IconItemSelectionWindow<T>(Vector2 size, bool fixedSize = true, string? alternativeName = null) : SelectionWindowBase<T>(size, fixedSize, alternativeName) where T : notnull {
	protected abstract float IconWidth { get; }

	protected abstract void DrawIcon(T option);
	protected abstract void DrawText(T option);

	protected override void DrawSelection(T option) {
		using (var imageContainer = ImRaii.Child("image_container", ImGuiHelpers.ScaledVector2(IconWidth, ImGui.GetContentRegionAvail().Y), false, ImGuiWindowFlags.NoInputs)) {
			if (!imageContainer) return;

			DrawIcon(option);
		}
		
		ImGui.SameLine();
		
		using (var textContainer = ImRaii.Child("text_container", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.NoInputs)) {
			if (!textContainer) return;
			
			DrawText(option);
		}
	}
}