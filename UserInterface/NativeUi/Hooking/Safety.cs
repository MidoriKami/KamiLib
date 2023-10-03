using System;
using System.Diagnostics;
using System.Reflection;

namespace KamiLib.NativeUi;

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

                Service.Log.Error($"Exception Source: {callingAssembly} :: {callingClass} :: {callingName}");
            }

            Service.Log.Error(exception, message ?? "Caught Exception Safely");
        }
    }
}