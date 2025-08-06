using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;

namespace KamiLib.Extensions;

public static class TextureProviderExtensions {
	public static void DrawGameIcon(this ITextureProvider textureProvider, uint iconId) {
		var iconWrap = textureProvider.GetFromGameIcon(new GameIconLookup(iconId)).GetWrapOrEmpty();
		
		ImGui.Image(iconWrap.Handle, iconWrap.Size * ImGuiHelpers.GlobalScale);
	}

	public static void DrawGameIcon(this ITextureProvider textureProvider, int iconId)
		=> textureProvider.DrawGameIcon((uint)iconId);
}