using Dalamud.Plugin.Services;

namespace KamiLib.Extensions;

public static class GameGuiExtensions {
    public static unsafe T* GetAddonByName<T>(this IGameGui gameGui, string addonName) where T : unmanaged
        => (T*) gameGui.GetAddonByName(addonName);
}