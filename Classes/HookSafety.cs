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
            var trace = new StackTrace().GetFrame(1);
            var callingAssembly = Assembly.GetCallingAssembly().GetName().Name;

            if (trace is not null) {
                var callingClass = trace.GetMethod()?.DeclaringType;
                var callingName = trace.GetMethod()?.Name;

                log.Error($"Exception Source: {callingAssembly} :: {callingClass} :: {callingName}");
            }

            log.Error(exception, message ?? "Caught Exception Safely");
        }
    }
}