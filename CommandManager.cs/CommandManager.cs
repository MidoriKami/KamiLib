using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dalamud.Game.Command;
using Dalamud.Plugin.Services;

namespace KamiLib.CommandManager;

public class CommandManager : IDisposable {
    private readonly ICommandManager dalamudCommandManager;
    private readonly IPluginLog log;

    private readonly List<CommandHandler> registeredCommands = [];
    private readonly List<string> registeredBaseCommands = [];

    public CommandManager(ICommandManager dalamudCommandManager, IPluginLog log, params string[] baseCommands) {
        this.dalamudCommandManager = dalamudCommandManager;
        this.log = log;

        foreach (var alias in baseCommands) {
            registeredBaseCommands.Add(alias);
            
            dalamudCommandManager.AddHandler($"/{alias}", new CommandInfo(DalamudCommandHandler) {
                ShowInHelp = false,
            });

            dalamudCommandManager.AddHandler($"/{alias} help", new CommandInfo(HelpCommandHandler) {
                ShowInHelp = true,
                HelpMessage = "Display all available commands",
            });
        }
    }

    public void Dispose() {
        foreach (var command in registeredBaseCommands) {
            dalamudCommandManager.RemoveHandler($"/{command}");
            dalamudCommandManager.RemoveHandler($"/{command} help");
        }
    }

    public void RegisterCommand(CommandHandler commandInfo) 
        => registeredCommands.Add(commandInfo);

    public void RemoveCommand(CommandHandler commandInfo) 
        => registeredCommands.Remove(commandInfo);

    public void RemoveCommand(string commandPath) 
        => registeredCommands.RemoveAll(command => string.Equals(command.ActivationPath, commandPath, StringComparison.OrdinalIgnoreCase));

    private void DalamudCommandHandler(string command, string arguments) {
        log.Verbose($"Received Command: {command}, Args: {arguments}");

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
            log.Error(e, $"Exception processing commands, Command: {command}, Arguments: {arguments}");
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
                    
            log.Verbose($"Command Delegate Executed. {commandPath}, {string.Join(" ", argumentsList)}");
        }
    }

    private void HelpCommandHandler(string command, string arguments) {
        log.Verbose($"Received Help Command: {command}, Args: {arguments}");
    }
}