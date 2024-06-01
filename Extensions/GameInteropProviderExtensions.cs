using System;
using Dalamud.Hooking;
using Dalamud.Plugin.Services;

namespace KamiLib.Extensions;

public static class GameInteropProviderExtensions {
    public static Hook<T> HookFromAddress<T>(this IGameInteropProvider provider, nuint address, T function) where T : Delegate 
        => provider.HookFromAddress((nint) address, function);
    
    public static unsafe Hook<T> HookFromAddress<T>(this IGameInteropProvider provider, void* address, T function) where T : Delegate 
        => provider.HookFromAddress((nint) address, function);
}