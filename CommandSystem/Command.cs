using System.Collections.Generic;
using System.Linq;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace KamiLib.CommandSystem;

public static class Command
{
    public static CommandData GetCommandData(string baseCommand, string arguments) => new(baseCommand, arguments);

    public static void ProcessCommand(CommandData data, IEnumerable<IPluginCommand> commands)
    {
        var matchingCommands = commands.Where(command => command.CommandArgument == data.Command).ToList();

        if (matchingCommands.Any())
        {
            var anyExecuted = false;

            foreach (var _ in matchingCommands.Where(cmd => cmd.Execute(data)))
            {
                anyExecuted = true;
            }

            if (!anyExecuted)
            {
                Chat.PrintError(string.Format(Strings.Command_DoesntExistExtended, data.BaseCommand, data.Command, data.SubCommand));
            }
        }
        else
        {
            Chat.PrintError(string.Format(Strings.Command_DoesntExist, data.BaseCommand, data.Command));
        }
    }
}