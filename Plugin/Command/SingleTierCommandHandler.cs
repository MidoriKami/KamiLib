using Dalamud.Game.Text.SeStringHandling;

namespace KamiLib.Command;

/// <summary>
///     Standard Commands with No Arguments<br />
///     Example: <b>/dailyduty help</b>
/// </summary>
public class SingleTierCommandHandler : CommandAttribute
{

    public SingleTierCommandHandler(string helpText, params string[] commands) : base(helpText)
    {
        Commands = commands;
    }
    public string[] Commands { get; init; }

    public override void AddFormattedHelpText(SeStringBuilder builder)
    {
        AddMultipleCommand(builder, Commands);
    }
}