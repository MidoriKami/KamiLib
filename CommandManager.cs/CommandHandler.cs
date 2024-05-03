namespace KamiLib.CommandManager;

public class CommandHandler {
    public delegate void CommandHandlerDelegate(params string[] args);
    
    public required string ActivationPath { get; set; }
    public required CommandHandlerDelegate Delegate { get; set; }

    public string? HelpText { get; set; }
    public bool Hidden { get; set; }
    
    internal bool Processed { get; set; }
}