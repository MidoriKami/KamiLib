using System;
using System.Numerics;
using Dalamud.Plugin;
using ImGuiNET;

namespace KamiLib.Window;

[Flags]
public enum WindowFlags {
    None = 0,
    OpenImmediately = 1,
    IsConfigWindow = 2,
    RequireLoggedIn = 4,
}

public abstract class Window : Dalamud.Interface.Windowing.Window {
    private bool isCollapsed;
    public IDalamudPluginInterface PluginInterface { get; set; } = null!;
    public WindowManager ParentWindowManager { get; set; } = null!;
    
    public string? AdditionalInfoTooltip { get; set; }
    
    public WindowFlags WindowFlags { get; set; }

    protected Window(string windowName, Vector2 size, bool fixedSize = false) : base(windowName) {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = size,
            MaximumSize = fixedSize ? size : new Vector2(float.PositiveInfinity),
        };

        if (fixedSize) {
            Flags |= ImGuiWindowFlags.NoResize;
        }

        Collapsed = false;
        CollapsedCondition = ImGuiCond.Always;
    }

    public virtual void Load() { }
    protected abstract void DrawContents();

    public void Open()
        => IsOpen = true;

    public void Close() 
        => IsOpen = false;

    public void UnCollapseOrShow() {
        if (isCollapsed) {
            isCollapsed = false;
            Collapsed = false;
            IsOpen = true;
        } else {
            IsOpen = true;
        }
    }

    public void UnCollapseOrToggle() {
        if (isCollapsed) {
            isCollapsed = false;
            Collapsed = false;
            IsOpen = true;
        } else {
            Toggle();
        }
    }

    public override void Update() 
        => isCollapsed = true;

    public override void Draw() {
        isCollapsed = false;
        Collapsed = null;

        DrawContents();
    }
}