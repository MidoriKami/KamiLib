using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Windowing;

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

    public void AddWindow(Window window)
    {
        windows.Add(window);
        windowSystem.AddWindow(window);
    }

    public T? GetWindowOfType<T>() => windows.OfType<T>().FirstOrDefault();
    private void DrawUI() => windowSystem.Draw();
    private void DrawConfigUI() => KamiCommon.CommandManager.OnCommand($"{KamiCommon.PluginName}", "");
}