using Dalamud.Game.ClientState;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Plugin;
using DalamudCommandManager = Dalamud.Game.Command.CommandManager;
using DalamudCondition = Dalamud.Game.ClientState.Conditions.Condition;

namespace KamiLib;

public class Service
{
    [PluginService] public static DalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] public static DalamudCommandManager Commands { get; private set; } = null!;
    [PluginService] public static ClientState ClientState { get; private set; } = null!;
    [PluginService] public static ChatGui Chat { get; private set; } = null!;
    [PluginService] public static GameGui GameGui { get; private set; } = null!;
    [PluginService] public static DalamudCondition Condition { get; private set; } = null!;
}

public static class KamiLib
{
    public static void Initialize(DalamudPluginInterface pluginInterface, string pluginName)
    {
        pluginInterface.Create<Service>();
        Chat.SetPluginName(pluginName);
    }
}