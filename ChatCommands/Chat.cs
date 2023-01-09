using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using KamiLib.Localization;

namespace KamiLib.ChatCommands;

public static class Chat
{
    public static void Print(string tag, string message) => Service.Chat.Print(GetBaseString(tag, message).BuiltString);

    public static void PrintHelp(string command, string? helpText = null)
    {
        var message = GetBaseString(Strings.Command_Label, command);

        if (helpText is not null)
        {
            message.AddUiForeground("- " + helpText, 32);
        }

        Service.Chat.Print(message.BuiltString);
    }
    
    public static void Print(string tag, string message, DalamudLinkPayload? payload)
    {
        if (payload is null)
        {
            Print(tag, message);
            return;
        }
        
        Service.Chat.Print(GetBaseString(tag, message, payload).BuiltString);
    }

    public static void PrintError(string message) => Service.Chat.PrintError(GetBaseString(Strings.Common_Error, message).BuiltString);
    
    private static SeStringBuilder GetBaseString(string tag, string message, DalamudLinkPayload? payload = null)
    {
        if (payload is null)
        {
            return new SeStringBuilder()
                .AddUiForeground($"[{KamiCommon.PluginName}] ", 45)
                .AddUiForeground($"[{tag}] ", 62)
                .AddText(message);
        }
        else
        {
            return new SeStringBuilder()
                .AddUiForeground($"[{KamiCommon.PluginName}] ", 45)
                .AddUiForeground($"[{tag}] ", 62)
                .Add(payload)
                .AddUiForeground(message, 35)
                .Add(RawPayload.LinkTerminator);
        }
    }
}