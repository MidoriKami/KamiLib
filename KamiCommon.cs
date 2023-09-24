using System;
using Dalamud.Plugin;
using KamiLib.Caching;
using KamiLib.Commands;
using KamiLib.Localization;
using KamiLib.System;
using KamiLib.Windows;

namespace KamiLib;

public static class KamiCommon
{
    public static string PluginName { get; private set; } = string.Empty;
    public static WindowManager WindowManager { get; private set; } = null!;
    public static LocalizationWrapper? Localization { get; private set; }
    public static FontController FontManager { get; private set; } = null!;
    
    public static void Initialize(DalamudPluginInterface pluginInterface, string pluginName)
    {
        pluginInterface.Create<Service>();

        PluginName = pluginName;

        LocalizationManager.Instance.Initialize();

        FontManager = new FontController();
        WindowManager = new WindowManager();
    }

    public static void RegisterLocalizationHandler(Func<string, string?> handler) => Localization = new LocalizationWrapper
    {
        GetTranslatedString = handler,
    };

    public static void Dispose()
    {
        CommandController.UnregisterMainCommands();
        DebugWindow.Cleanup();
        WindowManager.Dispose();
        LocalizationManager.Cleanup();
    }
}
