using System.Collections.Generic;
using System.Globalization;
using Dalamud.Interface.Windowing;
using KamiLib.Interfaces;
using KamiLib.Localization;

namespace KamiLib.ChatCommands;

public class OpenWindowCommand<T> : IPluginCommand where T : Window
{
    public string? CommandArgument { get; }

    public OpenWindowCommand(string? commandArgument = null, bool silent = false, string? windowName = null, bool enableInDen = false)
    {
        CommandArgument = commandArgument?.ToLower();

        if (commandArgument is not null)
        {
            windowName ??= CultureInfo.CurrentCulture.TextInfo.ToTitleCase(commandArgument);
        }
        
        SubCommands = new List<ISubCommand>
        {
            new SubCommand
            {
                CommandKeyword = null,
                CommandAction = () => Chat.PrintError(string.Format(Strings.Command_PvPError, windowName)),
                CanExecute = () => enableInDen ? Service.ClientState.IsPvPExcludingDen : Service.ClientState.IsPvP,
                GetHelpText = () => string.Format(Strings.Command_OpenWindow, windowName),
                Hidden = silent,
            },
            new SubCommand
            {
                CommandKeyword = null,
                CommandAction = () =>
                {
                    if ( KamiCommon.WindowManager.GetWindowOfType<T>() is {} mainWindow )
                    {
                        if (!silent)
                        {
                            Chat.Print(Strings.Command_Label,
                                !mainWindow.IsOpen ? 
                                    string.Format(Strings.Command_OpeningWindow, windowName) : 
                                    string.Format(Strings.Command_ClosingWindow, windowName));
                        }

                        mainWindow.IsOpen = !mainWindow.IsOpen;
                    }
                    else
                    {
                        Chat.PrintError($"Something went wrong trying to open {windowName} Window");
                    }
                },
                CanExecute = () => enableInDen ? !Service.ClientState.IsPvPExcludingDen : !Service.ClientState.IsPvP,
                GetHelpText = () => string.Format(Strings.Command_OpenWindow, windowName),
                Hidden = silent,
            },
        };
    }
    
    public IEnumerable<ISubCommand> SubCommands { get; }
}