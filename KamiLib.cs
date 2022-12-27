using System;
using Dalamud.Logging;
using Dalamud.Plugin;
using KamiLib.BlacklistSystem;
using KamiLib.CommandSystem;
using KamiLib.WindowSystem;

namespace KamiLib;

public static class KamiLib
{
    public static string PluginName { get; private set; } = string.Empty;
    public static CommandManager CommandManager { get; private set; } = null!;
    public static WindowManager WindowManager { get; private set; } = null!;
    
    private static Action _saveConfigFunction = null!;

    public static void Initialize(DalamudPluginInterface pluginInterface, string pluginName, Action saveConfig)
    {
        PluginLog.Debug("Initializing");

        pluginInterface.Create<Service>();

        PluginName = pluginName;
        _saveConfigFunction = saveConfig;

        BlacklistDraw.PrimeSearch();

        CommandManager = new CommandManager();
        WindowManager = new WindowManager();
    }

    public static void Dispose()
    {
        CommandManager.Dispose();
        WindowManager.Dispose();
    }

    public static void SaveConfiguration() => _saveConfigFunction();
}