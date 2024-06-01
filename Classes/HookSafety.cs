using System;
using System.Diagnostics;
using System.Reflection;
using Dalamud.Plugin.Services;

namespace KamiLib.Classes;

public static class HookSafety {
    public static void ExecuteSafe(Action action, IPluginLog log, string? message = null) {
        try {
            action();
        }
        catch (Exception exception) {
            if (new StackTrace().GetFrame(1) is { } trace) {
                var callingClass = trace.GetMethod()?.DeclaringType;
                var callingName = trace.GetMethod()?.Name;

                log.Error($"Exception Source: {Assembly.GetCallingAssembly().GetName().Name} :: {callingClass} :: {callingName}");
            }

            log.Error(exception, message ?? "Caught Exception Safely");
        }
    }
}