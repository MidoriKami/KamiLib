using System;
using Dalamud.Plugin;
using KamiLib.Blacklist;
using KamiLib.Caching;
using KamiLib.ChatCommands;
using KamiLib.GameState;
using KamiLib.Localization;
using KamiLib.Teleporter;
using KamiLib.UserInterface;
using KamiLib.Windows;

namespace KamiLib;

public static class KamiCommon
{
    public static string PluginName { get; private set; } = string.Empty;
    public static CommandManager CommandManager { get; private set; } = null!;
    public static WindowManager WindowManager { get; private set; } = null!;
    
    private static Action _saveConfigFunction = null!;

    public static void Initialize(DalamudPluginInterface pluginInterface, string pluginName, Action saveConfig)
    {
        pluginInterface.Create<Service>();

        PluginName = pluginName;
        _saveConfigFunction = saveConfig;

        LocalizationManager.Instance.Initialize();
        
        BlacklistDraw.PrimeSearch();

        CommandManager = new CommandManager();
        WindowManager = new WindowManager();
    }

    public static void Dispose()
    {
        CommandManager.Dispose();
        WindowManager.Dispose();
        IconCache.Cleanup();
        GameUserInterface.Cleanup();
        DutyState.Cleanup();
        ChatPayloadManager.Cleanup();
        TeleportManager.Cleanup();
        LocalizationManager.Cleanup();
    }

    public static void SaveConfiguration() => _saveConfigFunction();
}