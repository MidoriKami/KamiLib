using System.Numerics;
using ImGuiNET;

namespace KamiLib.Extensions;

public static class ImGuiStylePtrExtensions {
	public static Vector4 GetColor(this ImGuiStylePtr style, ImGuiCol color) {
		return style.Colors[(int)color];
	}
	
	public static Vector4 GetFadedColor(this ImGuiStylePtr style, ImGuiCol color, float alpha) {
		return style.GetColor(color) with { W = alpha };
	}
}