using System.Collections.Generic;
using System.Linq;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace KamiLib.CommandSystem;

public class HelpCommands: IPluginCommand
{
    public string CommandArgument => "help";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = null,
            CommandAction = () =>
            {
                foreach (var command in KamiCommon.CommandManager.Commands)
                {
                    PrintSubCommands(command);
                }
            },
            GetHelpText = () => "Show this message"
        }
    };

    private static void PrintSubCommands(IPluginCommand command)
    {
        foreach (var subCommand in command.SubCommands.GroupBy(subCommand => subCommand.GetCommand()))
        {
            var selectedSubCommand = subCommand.First();

            if (!selectedSubCommand.Hidden)
            {
                PrintHelpText(command, selectedSubCommand);

                if (selectedSubCommand.GetAliases() is { } aliases)
                {
                    foreach (var alias in aliases)
                    {
                        PrintAliasHelpText(command, alias, selectedSubCommand);
                    }
                }
            }
        }
    }

    private static void PrintHelpText(IPluginCommand mainCommand, ISubCommand subCommand)
    {
        var commandString = $"/{KamiCommon.PluginName.ToLower()} ";

        if (mainCommand.CommandArgument is not null)
        {
            commandString += mainCommand.CommandArgument + " ";
        }

        if (subCommand.GetCommand() is not null)
        {
            commandString += subCommand.GetCommand() + " ";
        }

        if (subCommand.HasParameterAction)
        {
            commandString += "[value] ";
        }
        
        Chat.PrintHelp(commandString, subCommand.GetHelpText());
    }

    private static void PrintAliasHelpText(IPluginCommand mainCommand, string? alias, ISubCommand subCommand)
    {
        var commandString = $"/{KamiCommon.PluginName.ToLower()} ";

        if (mainCommand.CommandArgument is not null)
        {
            commandString += mainCommand.CommandArgument + " ";
        }

        if (alias is not null)
        {
            commandString += alias + " ";
        }
        
        if (subCommand.HasParameterAction)
        {
            commandString += "[value] ";
        }
        
        Chat.PrintHelp(commandString, subCommand.GetHelpText());
    }
}