using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Windowing;
using KamiLib.ChatCommands;

namespace KamiLib.Windows;

public class WindowManager : IDisposable
{
    private readonly WindowSystem windowSystem;

    private readonly List<Window> windows = new();

    public WindowManager()
    {
        windowSystem = new WindowSystem(KamiCommon.PluginName);
        
        windows.ForEach(window => windowSystem.AddWindow(window));
        
        Service.PluginInterface.UiBuilder.Draw += DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }
    
    public void Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUI;

        windowSystem.RemoveAllWindows();
    }

    public void AddWindow(Window newWindow)
    {
        if (windowSystem.Windows.All(w => w.WindowName != newWindow.WindowName))
        {
            windows.Add(newWindow);
            windowSystem.AddWindow(newWindow);
        }
    }

    public void AddConfigurationWindow<T>(T configWindow, bool allowInDen = false) where T : Window
    {
        windows.Add(configWindow);
        windowSystem.AddWindow(configWindow);
        
        KamiCommon.CommandManager.AddCommand(new OpenWindowCommand<T>(null, false, "Configuration", allowInDen));
        KamiCommon.CommandManager.AddCommand(new OpenWindowCommand<T>("silent", true, "Configuration", allowInDen));
        
        Service.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;
    }

    public void RemoveWindow(Window window)
    {
        windows.Remove(window);
        windowSystem.RemoveWindow(window);
    }

    public IReadOnlyCollection<Window> GetWindows() => windows;

    public T? GetWindowOfType<T>() => windows.OfType<T>().FirstOrDefault();
    private void DrawUI() => windowSystem.Draw();
    private void DrawConfigUI() => KamiCommon.CommandManager.OnCommand($"{KamiCommon.PluginName}", "silent");
}