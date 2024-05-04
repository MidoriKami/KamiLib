namespace KamiLib.CommandManager;


public class CommandHandler {
    public required string ActivationPath { get; init; }
    public required CommandHandlerDelegate Delegate { get; init; }
    public bool Hidden { get; init; }
    internal bool Processed { get; set; }
}