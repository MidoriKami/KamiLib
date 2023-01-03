using System.Collections.Generic;
using System.Linq;
using Dalamud.Utility;
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
            foreach (var cmd in matchingCommands)
            {
                cmd.Execute(data);
            }
        }
        else
        {
            Chat.PrintError(Strings.Command_DoesntExist.Format(data.BaseCommand ?? string.Empty, data.Command ?? string.Empty));
        }
    }
}