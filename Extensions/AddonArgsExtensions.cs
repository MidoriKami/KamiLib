using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;

namespace KamiLib.Extensions;

public static unsafe class AddonArgsExtensions {
	public static T* GetAddon<T>(this AddonArgs args) where T : unmanaged
		=> (T*) args.Addon.Address;
}