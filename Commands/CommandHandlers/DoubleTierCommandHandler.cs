using Dalamud.Game.Text.SeStringHandling;
using KamiLib.Commands.Abstracts;

namespace KamiLib.Commands;

/// <summary>
/// Complex Commands with Arguments<br/>
/// Example: <b>/dailyduty tododisplay enable</b>
/// </summary>
public class DoubleTierCommandHandler : CommandAttribute
{
    public string Command { get; init; }
    public string[] SubCommands { get; init; }
    
    public DoubleTierCommandHandler(string helpText, string command, params string[] subCommands) : base(helpText)
    {
        Command = command;
        SubCommands = subCommands;
    }

    public override void AddFormattedHelpText(SeStringBuilder builder) => AddMultipleCommand(builder, SubCommands);
}