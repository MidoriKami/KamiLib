using Dalamud.Game.Text.SeStringHandling;

namespace KamiLib;

internal static class Chat
{
    private static string _pluginName = "Name Not Set";
    public static void SetPluginName(string pluginName) => _pluginName = pluginName;
    
    public static void Print(string tag, string message)
    {
        Service.Chat.Print(GetBaseString(tag, message).BuiltString);
    }
    
    public static void PrintHelp(string command, string? helpText = null)
    {
        var message = GetBaseString("Command", command);

        if (helpText is not null)
        {
            message.AddUiForeground("- " + helpText, 32);
        }

        Service.Chat.Print(message.BuiltString);
    }

    public static void PrintError(string message)
    {
        Service.Chat.PrintError(GetBaseString("Error", message).BuiltString);
    }
    
    private static SeStringBuilder GetBaseString(string tag, string message)
    {
        return new SeStringBuilder()
            .AddUiForeground($"[{_pluginName}] ", 45)
            .AddUiForeground($"[{tag}] ", 62)
            .AddText(message);
    }
}