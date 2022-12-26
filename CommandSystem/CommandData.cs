namespace KamiLib.CommandSystem;

public class CommandData
{
    public string? BaseCommand;
    public string? Command;
    public string? SubCommand;
    public string?[]? Arguments;
    
    public CommandData(string rootCommand, string arguments)
    {
        BaseCommand = rootCommand;
        
        if (arguments != string.Empty)
        {
            var splits = arguments.Split(' ');

            if (splits.Length >= 1)
            {
                Command = splits[0];
            }

            if (splits.Length >= 2)
            {
                SubCommand = splits[1];
            }

            if (splits.Length >= 3)
            {
                Arguments = splits[2..];
            }
        }
    }

    public override string ToString() => $"{Command ?? "Empty Command"}, " +
                                         $"{SubCommand ?? "Empty SubCommand"}, " +
                                         $"{(Arguments is null ? "Empty Args" : string.Join(", ", Arguments))}";
}