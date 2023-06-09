using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.AutomaticUserInterface;

namespace KamiLib.Commands.Abstracts;

public abstract class CommandAttribute : FunctionAttributeBase
{
    private readonly string? helpTextKey;

    public string HelpText => TryGetLocalizedString(helpTextKey);

    protected CommandAttribute(string helpText)
    {
        helpTextKey = helpText;
    }

    public virtual void AddFormattedHelpText(SeStringBuilder builder) { }
    protected void AddColoredCommand(SeStringBuilder builder, string command) => builder.AddUiForeground(command, 37);
    protected void AddIndentSpacing(SeStringBuilder builder) => builder.AddText(new string(' ', 8));
    protected void AddHelpText(SeStringBuilder builder, string helpText) => builder.AddText($" - {helpText}");
    
    protected void AddMultipleCommand(SeStringBuilder stringBuilder, string[] commands)
    {
        for (var i = 0; i < commands.Length; ++i)
        {
            AddIndentSpacing(stringBuilder);
            AddColoredCommand(stringBuilder, commands[i]);
            AddHelpText(stringBuilder, HelpText);
            if (i != commands.Length - 1) stringBuilder.Add(new NewLinePayload());
        }
    }
}