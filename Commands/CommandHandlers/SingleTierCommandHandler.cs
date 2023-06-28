using Dalamud.Game.Text.SeStringHandling;
using KamiLib.Commands.Abstracts;

namespace KamiLib.Commands;

/// <summary>
/// Standard Commands with No Arguments<br/>
/// Example: <b>/dailyduty help</b>
/// </summary>
public class SingleTierCommandHandler : CommandAttribute
{
    public string[] Commands { get; init; }

    public SingleTierCommandHandler(string helpText, params string[] commands) : base(helpText)
    {
        Commands = commands;
    }

    public override void AddFormattedHelpText(SeStringBuilder builder) => AddMultipleCommand(builder, Commands);
}