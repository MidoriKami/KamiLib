using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Common.Math;
using ImGuiNET;

namespace KamiLib.Window;

public class WindowManager : IDisposable {
    private readonly WindowSystem windowSystem;
    private readonly IDalamudPluginInterface pluginInterface;

    [PluginService] private IClientState ClientState { get; set; } = null!;

    [PluginService] private IChatGui ChatGui { get; set; } = null!;

    [PluginService] private IPluginLog PluginLog { get; set; } = null!;
    
    private Window? configWindow;

    private List<Window> Windows { get; } = [];
    
    public WindowManager(IDalamudPluginInterface pluginInterface) {
        windowSystem = new WindowSystem(pluginInterface.Manifest.InternalName);
        
        this.pluginInterface = pluginInterface;
        this.pluginInterface.Inject(this);
        
        pluginInterface.UiBuilder.Draw += windowSystem.Draw;
    }

    public void Dispose() {
        pluginInterface.UiBuilder.Draw -= windowSystem.Draw;
        pluginInterface.UiBuilder.OpenConfigUi -= OpenConfigurationWindow;

        windowSystem.RemoveAllWindows();
    }

    public void AddWindow(Window window, WindowFlags? windowFlags = null) {
        if (Windows.Any(existingWindow => string.Equals($"{window.WindowName}##{windowSystem.Namespace}", existingWindow.WindowName, StringComparison.OrdinalIgnoreCase))) return;
        
        window.PluginInterface = pluginInterface;
        window.ParentWindowManager = this;
        window.WindowName += $"##{pluginInterface.InternalName}";

        if (windowFlags is { } flags) {
            window.WindowFlags = flags;
        }
        
        pluginInterface.Inject(window);

        window.TitleBarButtons.Add(new Dalamud.Interface.Windowing.Window.TitleBarButton {
            Icon = FontAwesomeIcon.Question,
            ShowTooltip = () => ImGui.SetTooltip($"Window by {pluginInterface.InternalName}{(window.AdditionalInfoTooltip is not null ? "\n\n" : "")}{window.AdditionalInfoTooltip}"),
            IconOffset = new Vector2(3.0f, 1.0f),
            Priority = -1,
        });

        if (window.WindowFlags.HasFlag(WindowFlags.OpenImmediately)) {
            var isLoggedIn = ClientState.IsLoggedIn;
            var requiresLoggedIn = window.WindowFlags.HasFlag(WindowFlags.RequireLoggedIn);
            var disallowInPvP = !window.WindowFlags.HasFlag(WindowFlags.AllowInPvP);
            var isInPvP = ClientState.IsPvP;

            var loginCheckPass = !requiresLoggedIn || (requiresLoggedIn && isLoggedIn);
            var pvpCheckPass = disallowInPvP || (!disallowInPvP && isInPvP);

            if (loginCheckPass && pvpCheckPass) {
                window.UnCollapseOrShow();
            }
        }
        
        if (window.WindowFlags.HasFlag(WindowFlags.IsConfigWindow)) {
            configWindow = window;
            pluginInterface.UiBuilder.OpenConfigUi += OpenConfigurationWindow;
        }
        
        window.Load();

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

    public void ToggleWindow<T>(bool forceShow = false) where T : Window {
        if (forceShow) {
            GetWindow<T>()?.UnCollapseOrShow();
        }
        else {
            GetWindow<T>()?.UnCollapseOrToggle();
        }
    }

    private void OpenConfigurationWindow() {
        if (configWindow is null) {
            pluginInterface.UiBuilder.OpenConfigUi -= OpenConfigurationWindow;
            throw new Exception("Configuration Window Was Null, disabling ConfigUI Callback.");
        }

        if (!configWindow.WindowFlags.HasFlag(WindowFlags.AllowInPvP) && ClientState.IsPvP) {
            ChatGui.PrintError("The configuration menu cannot be opened while in a PvP area", pluginInterface.InternalName, 45);
            return;
        }

        if (configWindow.WindowFlags.HasFlag(WindowFlags.RequireLoggedIn) && !ClientState.IsLoggedIn) {
            return;
        }
        
        configWindow.UnCollapseOrShow();
    }
}