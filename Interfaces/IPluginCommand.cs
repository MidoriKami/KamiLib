using System.Collections.Generic;
using System.Linq;
using KamiLib.ChatCommands;

namespace KamiLib.Interfaces;

public interface IPluginCommand
{
    string? CommandArgument { get; }
    
    IEnumerable<ISubCommand> SubCommands { get; }
    
    public bool Execute(CommandData data)
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

            return true;
        }
        
        return false;
    }

    private static bool MatchingSubCommand(ISubCommand subCommand, string? targetCommand)
    {
        if (subCommand.GetCommand() == targetCommand) return true;
        if (subCommand.GetAliases()?.Contains(targetCommand) ?? false) return true;

        return false;
    }

}