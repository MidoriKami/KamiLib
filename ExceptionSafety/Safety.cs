using System;
using System.Diagnostics;
using System.Reflection;
using Dalamud.Logging;

namespace KamiLib.ExceptionSafety;

public static class Safety
{
    public static void ExecuteSafe(Action action, string? message = null) 
    {
        try 
        {
            action();
        } 
        catch (Exception exception)
        {
            var trace = new StackTrace().GetFrame(1);
            var callingAssembly = Assembly.GetCallingAssembly().GetName().Name;
            
            if (trace is not null)
            {
                var callingClass = trace.GetMethod()?.DeclaringType;
                var callingName = trace.GetMethod()?.Name;
                
                PluginLog.Error($"Exception Source: {callingAssembly} :: {callingClass} :: {callingName}");
            }

            PluginLog.Error(exception, message ?? "Caught Exception Safely");
        }
    }
}