using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Dalamud.Game.Command;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using KamiLib.ChatCommands;
using KamiLib.Commands.Abstracts;
using KamiLib.Commands.temp;
using KamiLib.Localization;

namespace KamiLib.Commands;

public static class CommandController
{
    private delegate void BaseCommandDelegate();
    private delegate void SingleTierCommandDelegate();
    private delegate void DoubleTierCommandDelegate(params string[] args);

    private record DelegateInfo<T, TU>(T Delegate, TU Attribute) where T : Delegate where TU : CommandAttribute;

    private static readonly List<DelegateInfo<BaseCommandDelegate, BaseCommandHandler>> BaseCommands = new();
    private static readonly List<DelegateInfo<SingleTierCommandDelegate, SingleTierCommandHandler>> SingleTierCommands = new();
    private static readonly List<DelegateInfo<DoubleTierCommandDelegate, DoubleTierCommandHandler>> DoubleTierCommands = new();

    private static readonly List<string> MainCommands = new();
    
    public static void RegisterMainCommand(string command, params string[] aliases)
    {
        foreach (var alias in aliases.Prepend(command))
        {
            MainCommands.Add(alias);
            Service.Commands.AddHandler(alias, new CommandInfo(CommandHandler)
            {
                ShowInHelp = false,
            });
        }
        
        SingleTierCommands.Add(new DelegateInfo<SingleTierCommandDelegate, SingleTierCommandHandler>(PrintHelpText, new SingleTierCommandHandler("HelpText", "help")));
    }

    public static void UnregisterMainCommands()
    {
        foreach (var command in MainCommands)
        {
            Service.Commands.RemoveHandler(command);
        }
    }
    
    private static void CommandHandler(string command, string arguments)
    {
#if DEBUG
        PluginLog.Debug(string.IsNullOrEmpty(arguments) ? $"Received Command: {command}" : $"Received Command: {command}, {string.Join(", ", arguments.Split(" "))}");
#endif
        var totalCommandCount = BaseCommands.Count + SingleTierCommands.Count + DoubleTierCommands.Count;
        if (totalCommandCount is 0)
        {
            Chat.PrintError("No Commands Registered.");
            return;
        }

        if (!string.IsNullOrEmpty(command) && string.IsNullOrEmpty(arguments))
        {
            foreach (var (baseDelegate, _) in BaseCommands)
            {
                baseDelegate.Invoke();
            }
        }

        if (!string.IsNullOrEmpty(command) && !string.IsNullOrEmpty(arguments))
        {
            var argumentArray = arguments.Split(" ");
            var firstCommand = argumentArray[0];
        
            if (argumentArray.Length >= 2)
            {
                var secondCommand = argumentArray[1];

                foreach (var (secondDelegate, secondAttribute) in DoubleTierCommands)
                {
                    if (string.Equals(secondAttribute.Command, firstCommand, StringComparison.OrdinalIgnoreCase))
                    {
                        foreach (var subCommand in secondAttribute.SubCommands)
                        {
                            if (string.Equals(secondCommand, subCommand, StringComparison.OrdinalIgnoreCase))
                            {
                                secondDelegate.Invoke(argumentArray[2..]);
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (var (firstDelegate, firstAttribute) in SingleTierCommands)
                {
                    foreach (var subCommand in firstAttribute.Commands)
                    {
                        if (string.Equals(firstCommand, subCommand, StringComparison.OrdinalIgnoreCase))
                        {
                            firstDelegate.Invoke();
                        }
                    }
                }
            }
        }
    }

    public static void RegisterCommands(object obj)
    {
        var methods = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var method in methods)
        {
            if (TryGetAttribute<BaseCommandHandler>(method, out var baseCommandHandler))
            {
                BaseCommands.Add(new DelegateInfo<BaseCommandDelegate, BaseCommandHandler>(method.CreateDelegate<BaseCommandDelegate>(obj), baseCommandHandler));
            }
            
            if (TryGetAttribute<SingleTierCommandHandler>(method, out var singleTierCommandHandler))
            {
                SingleTierCommands.Add(new DelegateInfo<SingleTierCommandDelegate, SingleTierCommandHandler>(method.CreateDelegate<SingleTierCommandDelegate>(obj), singleTierCommandHandler));
            }
            
            if (TryGetAttribute<DoubleTierCommandHandler>(method, out var attribute))
            {
                DoubleTierCommands.Add(new DelegateInfo<DoubleTierCommandDelegate, DoubleTierCommandHandler>(method.CreateDelegate<DoubleTierCommandDelegate>(obj), attribute));
            }
        }
    }

    private static void PrintHelpText()
    {
        Chat.Print(Strings.Command_Label, "Displaying all available commands");

        var stringBuilder = new SeStringBuilder();
        stringBuilder.Add(new NewLinePayload());
        
        AddBaseCommands(stringBuilder);

        AddSingleTierCommands(stringBuilder);

        AddDoubleTierCommands(stringBuilder);

        Service.Chat.Print(stringBuilder.Build());
    }
    private static void AddDoubleTierCommands(SeStringBuilder stringBuilder)
    {
        if (DoubleTierCommands.Count > 0)
        {
            var uniqueCommands = DoubleTierCommands.Select(info => info.Attribute.Command).ToHashSet();

            foreach (var uniqueCommand in uniqueCommands)
            {
                var headerCommands = MainCommands.Select(command => $"{command} {uniqueCommand}");
                AddCommandHeader(stringBuilder, headerCommands.ToArray());

                foreach (var handler in DoubleTierCommands.Where(info => info.Attribute.Command == uniqueCommand))
                {
                    handler.Attribute.AddFormattedHelpText(stringBuilder);
                    stringBuilder.Add(new NewLinePayload());
                }
            }
        }
    }
    
    private static void AddSingleTierCommands(SeStringBuilder stringBuilder)
    {
        if (SingleTierCommands.Count > 0)
        {
            AddCommandHeader(stringBuilder, MainCommands.ToArray());

            foreach (var (_, singleTierAttribute) in SingleTierCommands)
            {
                singleTierAttribute.AddFormattedHelpText(stringBuilder);
                stringBuilder.Add(new NewLinePayload());
            }
        }
    }
    
    private static void AddBaseCommands(SeStringBuilder stringBuilder)
    {
        if (BaseCommands.Count > 0)
        {
            foreach (var mainCommand in MainCommands)
            {
                foreach (var (_, baseCommandAttribute) in BaseCommands)
                {
                    stringBuilder.AddUiForeground($"{mainCommand}", 25);
                    stringBuilder.AddText($" - {baseCommandAttribute.HelpText}");
                    stringBuilder.Add(new NewLinePayload());
                }
            }
        }
    }

    private static void AddCommandHeader(SeStringBuilder stringBuilder, params string[] mainCommands)
    {
        stringBuilder.Add(new NewLinePayload());
        stringBuilder.AddText("Use ");

        var commandList = mainCommands.ToList();
        for (var i = 0; i < commandList.Count; ++i)
        {
            stringBuilder.AddUiForeground(commandList[i], 25);
            if (i != commandList.Count - 1) stringBuilder.AddText(" or ");
        }

        stringBuilder.AddText(" with one of the following arguments:");
        stringBuilder.Add(new NewLinePayload());
    }

    private static bool TryGetAttribute<T>(MemberInfo method, [NotNullWhen(true)] out T? attribute) where T : Attribute
    {
        attribute = null;
        
        if (method.IsDefined(typeof(T), true))
        {
            attribute = method.GetCustomAttribute<T>();
            if(attribute is not null) return true;
        }

        return false;
    }
}