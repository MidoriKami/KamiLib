using System.Collections.Generic;
using System.Globalization;
using Dalamud.Interface.Windowing;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace KamiLib.CommandSystem;

public class OpenWindowCommand<T> : IPluginCommand where T : Window
{
    public string? CommandArgument { get; }

    public OpenWindowCommand(string commandArgument, string? windowName = null)
    {
        CommandArgument = commandArgument.ToLower();

        windowName ??= CultureInfo.CurrentCulture.TextInfo.ToTitleCase(commandArgument);
        
        SubCommands = new List<ISubCommand>
        {
            new SubCommand
            {
                CommandKeyword = null,
                CommandAction = () => Chat.PrintError($"The {windowName} Window cannot be opened while in a PvP area"),
                CanExecute = () => Service.ClientState.IsPvP,
                GetHelpText = () => $"Open {windowName} Window"
            },
            new SubCommand
            {
                CommandKeyword = null,
                CommandAction = () =>
                {
                    if ( KamiCommon.WindowManager.GetWindowOfType<T>() is {} mainWindow )
                    {
                        Chat.Print("Command",!mainWindow.IsOpen ? $"Opening {windowName} Window" : $"Closing {windowName} Window");

                        mainWindow.IsOpen = !mainWindow.IsOpen;
                    }
                    else
                    {
                        Chat.PrintError($"Something went wrong trying to open {windowName} Window");
                    }
                },
                CanExecute = () => !Service.ClientState.IsPvP,
                GetHelpText = () => $"Open {windowName} Window"
            },
        };
    }
    
    public IEnumerable<ISubCommand> SubCommands { get; }
}