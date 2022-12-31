using System.Collections.Generic;
using Dalamud.Interface.Windowing;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace KamiLib.CommandSystem;

internal class SilentConfigurationWindowCommand<T> : IPluginCommand where T : Window
{
    public string CommandArgument => "silent";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () => Chat.PrintError("The configuration window cannot be opened while in a PvP area"),
            CanExecute = () => Service.ClientState.IsPvP,
            GetHelpText = () => "Open Configuration Window",
            Hidden = true
        },
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                if (KamiCommon.WindowManager.GetWindowOfType<T>() is { } mainWindow)
                {
                    mainWindow.IsOpen = !mainWindow.IsOpen;
                }
                else
                {
                    Chat.PrintError("Something went wrong trying to open Configuration Window");
                }
            },
            CanExecute = () => !Service.ClientState.IsPvP,
            GetHelpText = () => "Open Configuration Window",
            Hidden = true
        },
    };
}