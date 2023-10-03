using Dalamud.Game.Text.SeStringHandling;

namespace KamiLib.Command;

/// <summary>
///     Complex Commands with Arguments<br />
///     Example: <b>/dailyduty tododisplay enable</b>
/// </summary>
public class DoubleTierCommandHandler : CommandAttribute
{

    public DoubleTierCommandHandler(string helpText, string command, params string[] subCommands) : base(helpText)
    {
        Command = command;
        SubCommands = subCommands;
    }
    public string Command { get; init; }
    public string[] SubCommands { get; init; }

    public override void AddFormattedHelpText(SeStringBuilder builder)
    {
        AddMultipleCommand(builder, SubCommands);
    }
}