using System.Collections.Generic;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace KamiLib.CommandSystem;

internal class LocalizationCommand : IPluginCommand
{
    public string CommandArgument => "loc";

    public IEnumerable<ISubCommand> SubCommands { get; } = new List<ISubCommand>
    {
        new SubCommand
        {
            CommandKeyword = "generate",
            CommandAction = () =>
            {
                LocalizationManager.Instance.ExportLocalization();
                Chat.Print("Command", "Generating Localization File");
            },
            Hidden = true,
        },
    };
}