using System;
using KamiLib.Interfaces;

namespace KamiLib;

public class SubCommand : ISubCommand
{
    public string? CommandKeyword { get; init; }
    public Action? CommandAction { get; init; }
    public Action<string?[]?>? ParameterAction { get; init; }
    public Func<bool>? CanExecute { get; init; }
    public Func<string>? GetHelpText { get; init; }
    public bool Hidden { get; init; }

    public string? GetCommand() => CommandKeyword;
    string? ISubCommand.GetHelpText() => GetHelpText?.Invoke();

    public bool Execute(CommandData commandData)
    {
        if (CanExecute?.Invoke() is null or true)
        {
            if (CommandAction is not null)
            {
                CommandAction.Invoke();
                return true;
            }

            if (ParameterAction is not null)
            {
                ParameterAction.Invoke(commandData.Arguments);
                return true;
            }
        }

        return false;
    }
}