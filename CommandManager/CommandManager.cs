using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dalamud.Game.Command;
using Dalamud.Interface;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Text;

namespace KamiLib.CommandManager;

public delegate void CommandHandlerDelegate(params string[] args);

public class CommandManager : IDisposable {
    [PluginService] private ICommandManager DalamudCommandManager { get; set; } = null!;
    [PluginService] private IPluginLog Log { get; set; } = null!;
    [PluginService] private IChatGui ChatGui { get; set; } = null!;
    
    private readonly DalamudPluginInterface pluginInterface;

    private readonly List<CommandHandler> registeredCommands = [];
    private readonly List<string> registeredBaseCommands = [];

    public CommandManager(DalamudPluginInterface pluginInterface, params string[] baseCommands) {
        this.pluginInterface = pluginInterface;
        this.pluginInterface.Inject(this);
        
        foreach (var alias in baseCommands) {
            registeredBaseCommands.Add(alias);
            
            DalamudCommandManager.AddHandler($"/{alias} help", new CommandInfo(HelpCommandHandler) {
                ShowInHelp = true,
                HelpMessage = "Display all available commands",
            });
            
            DalamudCommandManager.AddHandler($"/{alias}", new CommandInfo(DalamudCommandHandler) {
                ShowInHelp = false,
            });
        }
    }

    public void Dispose() {
        foreach (var command in registeredBaseCommands) {
            DalamudCommandManager.RemoveHandler($"/{command}");
            DalamudCommandManager.RemoveHandler($"/{command} help");
        }
    }

    public void RegisterCommand(CommandHandler commandInfo) 
        => registeredCommands.Add(commandInfo);

    public void RegisterCommand(ToggleCommandHandler toggleCommandHandler) {
        registeredCommands.Add(toggleCommandHandler.GetCommandHandler(ToggleCommandSubCommand.Enable));
        registeredCommands.Add(toggleCommandHandler.GetCommandHandler(ToggleCommandSubCommand.Disable));
        registeredCommands.Add(toggleCommandHandler.GetCommandHandler(ToggleCommandSubCommand.Toggle));
    }

    public void RemoveCommand(CommandHandler commandInfo) 
        => registeredCommands.Remove(commandInfo);

    public void RemoveCommand(string commandPath) 
        => registeredCommands.RemoveAll(command => string.Equals(command.ActivationPath, commandPath, StringComparison.OrdinalIgnoreCase));

    private void DalamudCommandHandler(string command, string arguments) {
        Log.Verbose($"Received Command: {command}, Args: {arguments}");
        
        if (arguments is "help") {
            HelpCommandHandler(command, arguments);
            return;
        }
        
        var argumentsList = arguments.Split(" ").ToList();
        var currentPath = string.Empty;
        
        try {
            // Try to Execute Base Command Handlers
            TryExecuteCommand("/", argumentsList);
            
            foreach (var argument in argumentsList.ToArray()) {
                currentPath += $"/{argument}";
                argumentsList.Remove(argument);
            
                TryExecuteCommand(currentPath, argumentsList);
            }
        }
        catch (Exception e) {
            Log.Error(e, $"Exception processing commands, Command: {command}, Arguments: {arguments}");
        }
        finally {
            registeredCommands.ForEach(handler => handler.Processed = false);
        }
    }
    
    private void TryExecuteCommand(string commandPath, List<string> argumentsList) {
        var handlersForPath = registeredCommands
            .Where(handler => !handler.Processed)
            .Where(handler => string.Equals(handler.ActivationPath, commandPath, StringComparison.OrdinalIgnoreCase));
        
        foreach (var handler in handlersForPath) {
            handler.Delegate(argumentsList.ToArray());
            handler.Processed = true;
                    
            Log.Verbose($"Command Delegate Executed. {commandPath}, {string.Join(" ", argumentsList)}");
        }
    }

    private void HelpCommandHandler(string command, string arguments) {
        var stringBuilder = new SeStringBuilder()
            .PushColorRgba(KnownColor.ForestGreen.Vector())
            .Append($"[{pluginInterface.InternalName}] ")
            .PopColor()
            .PushColorRgba(KnownColor.Gold.Vector())
            .Append("[Command] ")
            .PopColor()
            .Append("Displaying all commands\n\n");

        foreach (var registeredCommand in registeredCommands) {
            if (!registeredCommand.Hidden) {
                stringBuilder.PushColorRgba(KnownColor.White.Vector());
                stringBuilder.Append($"/{registeredBaseCommands[0]}");

                foreach (var part in registeredCommand.ActivationPath.Replace("/", " ").Split(" ")) {
                    if (part.IsNullOrEmpty()) continue;
                    
                    stringBuilder.Append($" {part}");
                }
                
                stringBuilder.PopColor();
            }

            stringBuilder.Append("\n");
        }
        
        ChatGui.Print(stringBuilder.ToSeString().ToDalamudString());
    }
}