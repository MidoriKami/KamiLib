using System;
using Dalamud.Plugin;
using KamiLib.Caching;
using KamiLib.ChatCommands;
using KamiLib.Localization;
using KamiLib.Windows;

namespace KamiLib;

public static class KamiCommon
{
    public static string PluginName { get; private set; } = string.Empty;
    public static CommandManager CommandManager { get; private set; } = null!;
    public static WindowManager WindowManager { get; private set; } = null!;
    public static LocalizationWrapper? Localization { get; private set; }
    
    public static void Initialize(DalamudPluginInterface pluginInterface, string pluginName)
    {
        pluginInterface.Create<Service>();

        PluginName = pluginName;

        LocalizationManager.Instance.Initialize();
        
        CommandManager = new CommandManager();
        WindowManager = new WindowManager();
    }

    public static void RegisterLocalizationHandler(Func<string, string?> handler) => Localization = new LocalizationWrapper
    {
        GetTranslatedString = handler,
    };

    public static void Dispose()
    {
        CommandManager.Dispose();
        WindowManager.Dispose();
        IconCache.Cleanup();
        LocalizationManager.Cleanup();
    }
}
