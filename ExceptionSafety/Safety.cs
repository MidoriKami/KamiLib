using System;
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
            PluginLog.Error(exception, message ?? "Caught Exception Safely");
        }
    }
}