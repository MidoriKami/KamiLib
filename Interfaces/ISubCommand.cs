namespace KamiLib.Interfaces;

public interface ISubCommand
{
    string? GetCommand();
    bool Execute(CommandData commandData);
    string? GetHelpText();
    bool Hidden { get; }
}