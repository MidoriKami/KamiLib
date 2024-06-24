using System;

namespace KamiLib.CommandManager;

public enum ToggleCommandSubCommand {
    Enable,
    Disable,
    Toggle,
}

public class ToggleCommandHandler {
    public required string BaseActivationPath { get; init; }
    public required CommandHandlerDelegate EnableDelegate { get; init; }
    public required CommandHandlerDelegate DisableDelegate { get; init; }
    public required CommandHandlerDelegate ToggleDelegate { get; init; }
    public bool Hidden { get; set; }

    public CommandHandler GetCommandHandler(ToggleCommandSubCommand subCommand)
        => new CommandHandler {
            Delegate = subCommand switch {
                ToggleCommandSubCommand.Enable => EnableDelegate,
                ToggleCommandSubCommand.Disable => DisableDelegate,
                ToggleCommandSubCommand.Toggle => ToggleDelegate,
                _ => throw new ArgumentOutOfRangeException(nameof(subCommand), subCommand, null),
            },
            ActivationPath = BaseActivationPath + subCommand switch {
                ToggleCommandSubCommand.Enable => "/enable",
                ToggleCommandSubCommand.Disable => "/disable",
                ToggleCommandSubCommand.Toggle => "/toggle",
                _ => throw new ArgumentOutOfRangeException(nameof(subCommand), subCommand, null),
            },
            Hidden = Hidden,
        };
}