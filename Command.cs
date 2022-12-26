using System.Collections.Generic;
using System.Linq;
using KamiLib.Interfaces;

namespace KamiLib;

public static class Command
{
    public static CommandData GetCommandData(string baseCommand, string arguments) => new(baseCommand, arguments);

    public static void ProcessCommand(CommandData data, IEnumerable<IPluginCommand> commands)
    {
        var matchingCommands = commands.Where(command => command.CommandArgument == data.Command).ToList();

        if (matchingCommands.Any())
        {
            foreach (var cmd in matchingCommands)
            {
                cmd.Execute(data);
            }
        }
        else
        {
            Chat.PrintError($"The command '/{data.BaseCommand} {data.Command}' does not exist.");
        }
    }
}