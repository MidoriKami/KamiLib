using System.Collections.Generic;
using System.Linq;
using KamiLib.CommandSystem;
using KamiLib.Utilities;

namespace KamiLib.Interfaces;

public interface IPluginCommand
{
    string? CommandArgument { get; }
    
    IEnumerable<ISubCommand> SubCommands { get; }
    
    public void Execute(CommandData data)
    {
        var matchingSubCommands = SubCommands
            .Where(subCommand => MatchingSubCommand(subCommand, data.SubCommand))
            .ToList();

        if (matchingSubCommands.Any())
        {
            foreach (var subCommand in matchingSubCommands)
            {
                subCommand.Execute(data);
            }
        }
        else
        {
            Chat.PrintError(string.Format(Strings.Command_DoesntExistExtended, data.BaseCommand, data.Command, data.SubCommand));
        }
    }

    private static bool MatchingSubCommand(ISubCommand subCommand, string? targetCommand)
    {
        if (subCommand.GetCommand() == targetCommand) return true;
        if (subCommand.GetAliases()?.Contains(targetCommand) ?? false) return true;

        return false;
    }

}