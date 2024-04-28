using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace KamiLib.Window;

public class WindowManager : IDisposable {
    private readonly WindowSystem windowSystem;
    private readonly DalamudPluginInterface pluginInterface;
    
    private Window? configWindow;

    public List<Window> Windows { get; } = [];
    
    public WindowManager(DalamudPluginInterface pluginInterface) {
        windowSystem = new WindowSystem(pluginInterface.Manifest.InternalName);
        
        this.pluginInterface = pluginInterface;
        
        pluginInterface.UiBuilder.Draw += windowSystem.Draw;
    }

    public void Dispose() {
        pluginInterface.UiBuilder.Draw -= windowSystem.Draw;
        pluginInterface.UiBuilder.OpenConfigUi -= OpenConfigurationWindow;

        windowSystem.RemoveAllWindows();
    }

    public void AddWindow(Window window, bool isConfigWindow = false) {
        if (isConfigWindow) {
            configWindow = window;
            pluginInterface.UiBuilder.OpenConfigUi += OpenConfigurationWindow;
        }
        
        window.TitleBarButtons.Add(new Dalamud.Interface.Windowing.Window.TitleBarButton {
            Icon = FontAwesomeIcon.Question,
            ShowTooltip = () => ImGui.SetTooltip($"Window by {pluginInterface.InternalName}{(window.AdditionalInfoTooltip is not null ? "\n\n" : "")}{window.AdditionalInfoTooltip}"),
            IconOffset = new Vector2(3.0f, 1.0f),
        });
        
        windowSystem.AddWindow(window);
        Windows.Add(window);
    }

    public void RemoveWindow(Window window) {
        windowSystem.RemoveWindow(window);
        Windows.Remove(window);
    }

    public T? GetWindow<T>() where T : Window
        => Windows.OfType<T>().FirstOrDefault();
    
    public IEnumerable<T> GetWindows<T>() where T : Window
        => Windows.OfType<T>();

    public void ToggleWindow<T>() where T : Window
        => GetWindow<T>()?.UnCollapseOrToggle();

    private void OpenConfigurationWindow() {
        if (configWindow is null) {
            pluginInterface.UiBuilder.OpenConfigUi -= OpenConfigurationWindow;
            throw new Exception("Configuration Window Was Null, disabling ConfigUI Callback.");
        }
        
        configWindow.UnCollapseOrShow();
    }
}