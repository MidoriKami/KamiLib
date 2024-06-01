using System.Linq;
using Dalamud.Plugin.Services;
using Dalamud.Utility;

namespace KamiLib.CommandManager;


public class CommandHandler {
    public required string ActivationPath { get; init; }
    public required CommandHandlerDelegate Delegate { get; init; }
    public bool Hidden { get; init; }
    public int CommandLength => ActivationPath.Split("/").Prepend("/").Count(part => !part.IsNullOrEmpty());

    public bool CommandMatches(IPluginLog log, params string[] commandParts) {
        var thisCommandParts = ActivationPath.Split("/").Prepend("/").Where(part => !part.IsNullOrEmpty()).ToArray();

        return commandParts.Length == CommandLength && Enumerable.Range(0, CommandLength).All(index => thisCommandParts[index] == commandParts[index]);
    }
}