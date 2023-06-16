using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace KamiLib.Windows;

public class DebugWindow : Window
{
    private static DebugWindow? _instance;
    private static DebugWindow Instance => _instance ??= new DebugWindow();
    private readonly List<string> debugLines = new();

    private DebugWindow() : base("KamiLibDebug")
    {
        KamiCommon.WindowManager.AddWindow(this);
        
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(200, 300),
            MaximumSize = new Vector2(9999,9999),
        };

        IsOpen = true;
    }
    
    public static void Cleanup() => KamiCommon.WindowManager.RemoveWindow(Instance);

    public override void Draw()
    {
        foreach (var line in debugLines)
        {
            ImGui.TextUnformatted(line);
        }
        
        debugLines.Clear();
    }

    public static void Print(string message) => Instance.debugLines.Add(message);
}