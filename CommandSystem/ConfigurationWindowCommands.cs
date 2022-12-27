using System.Collections.Generic;
using Dalamud.Interface.Windowing;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace KamiLib.CommandSystem;

public class ConfigurationWindowCommands<T> : IPluginCommand where T : Window
{
    public string? CommandArgument => null;

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () => Chat.PrintError("The configuration window cannot be opened while in a PvP area"),
            CanExecute = () => Service.ClientState.IsPvP,
            GetHelpText = () => "Open Configuration Window"
        },
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                if ( KamiLib.WindowManager.GetWindowOfType<T>() is {} mainWindow )
                {
                    Chat.Print("Command",!mainWindow.IsOpen ? "Opening Configuration Window" : "Closing Configuration Window");

                    mainWindow.IsOpen = !mainWindow.IsOpen;
                }
                else
                {
                    Chat.PrintError("Something went wrong trying to open Configuration Window");
                }
            },
            CanExecute = () => !Service.ClientState.IsPvP,
            GetHelpText = () => "Open Configuration Window"
        },
    };
}