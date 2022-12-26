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
                foreach (var command in KamiLib.CommandManager.Commands)
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
            }
        }
    }

    private static void PrintHelpText(IPluginCommand mainCommand, ISubCommand subCommand)
    {
        var commandString = $"/{KamiLib.PluginName.ToLower()} ";

        if (mainCommand.CommandArgument is not null)
        {
            commandString += mainCommand.CommandArgument + " ";
        }

        if (subCommand.GetCommand() is not null)
        {
            commandString += subCommand.GetCommand() + " ";
        }

        Chat.PrintHelp(commandString, subCommand.GetHelpText());
    }
}