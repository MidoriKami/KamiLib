using System;
using System.IO;
using Dalamud.Interface.ManagedFontAtlas;
using Dalamud.Plugin;
using KamiLib.Configuration;

namespace KamiLib.Extensions;

public static class PluginInterfaceExtensions {
	internal static FileInfo GetProfilePictureFileInfo(this IDalamudPluginInterface pluginInterface, CharacterConfiguration characterConfiguration) 
		=> new(Path.Combine(pluginInterface.GetCharacterDirectoryInfo(characterConfiguration.ContentId).FullName, "profile.png"));

	public static IDisposable PushIconFont(this IDalamudPluginInterface pluginInterface)
		=> pluginInterface.GetIconFont().Push();

	public static IFontHandle GetIconFont(this IDalamudPluginInterface pluginInterface)
		=> pluginInterface.UiBuilder.IconFontFixedWidthHandle;
}