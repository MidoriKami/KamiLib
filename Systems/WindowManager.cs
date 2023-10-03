using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface.Windowing;
using KamiLib.Game;

namespace KamiLib.System;

public class WindowManager : IDisposable
{

    private readonly List<Window> windows = new();
    private readonly WindowSystem windowSystem;
    private Window? configurationWindow;
    private Action? openConfigWindow;

    public WindowManager()
    {
        windowSystem = new WindowSystem(KamiCommon.PluginName);

        windows.ForEach(window => windowSystem.AddWindow(window));

        Service.PluginInterface.UiBuilder.Draw += DrawUI;
    }

    public void Dispose()
    {
        Service.PluginInterface.UiBuilder.Draw -= DrawUI;
        Service.PluginInterface.UiBuilder.OpenConfigUi -= openConfigWindow;

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
        configurationWindow = configWindow;

        openConfigWindow = () =>
        {
            if (!allowInDen && Service.ClientState.IsPvP)
            {
                Chat.PrintError("The configuration menu cannot be opened while in a PvP area");
                return;
            }

            configurationWindow?.Toggle();
        };

        Service.PluginInterface.UiBuilder.OpenConfigUi += openConfigWindow;
    }

    public void RemoveWindow(Window window)
    {
        windows.Remove(window);
        windowSystem.RemoveWindow(window);
    }

    public IReadOnlyCollection<Window> GetWindows()
    {
        return windows;
    }

    public T? GetWindowOfType<T>()
    {
        return windows.OfType<T>().FirstOrDefault();
    }
    private void DrawUI()
    {
        windowSystem.Draw();
    }

    public void ToggleWindowOfType<T>() where T : Window
    {
        if (GetWindowOfType<T>() is { } window)
        {
            window.IsOpen = !window.IsOpen;
        }
    }
}