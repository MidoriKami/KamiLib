using System;
using System.Linq;
using System.Reflection;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.Attributes;

namespace KamiLib.Extensions;

public static class GameGuiExtensions {
    [Obsolete("Don't forget to switch this to Dalamud.IGameGui.GetAddonByName, after the next dalamud stable release.")]
    public static unsafe T* InternalGetAddonByName<T>(this IGameGui gameGui, string addonName) where T : unmanaged
        => (T*) gameGui.GetAddonByName(addonName).Address;

    public static unsafe T* GetAddon<T>(this IGameGui gameGui) where T : unmanaged {
        var type = typeof(T);
        var attribute = type.GetCustomAttributes().OfType<AddonAttribute>().FirstOrDefault();
        var addonName = attribute?.AddonIdentifiers.FirstOrDefault();
        
        if (addonName is null) return null;
        return (T*) gameGui.GetAddonByName(addonName).Address;
    }
}
