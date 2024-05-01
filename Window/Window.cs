using System.Numerics;
using Dalamud.Plugin;
using ImGuiNET;

namespace KamiLib.Window;

public abstract class Window : Dalamud.Interface.Windowing.Window {
    private bool isCollapsed;
    public DalamudPluginInterface PluginInterface { get; set; } = null!;
    public WindowManager ParentWindowManager { get; set; } = null!;
    
    public string? AdditionalInfoTooltip { get; set; }

    protected Window(string windowName, Vector2 size, bool fixedSize = false) : base(windowName) {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = size,
            MaximumSize = fixedSize ? size : new Vector2(float.PositiveInfinity),
        };

        if (fixedSize) {
            Flags |= ImGuiWindowFlags.NoResize;
        }
    }

    public virtual void PrintOpenNotAllowed() { }

    public virtual bool IsOpenAllowed() => true;
    
    /// <summary>
    /// For loading assets that depend on injected dalamud services.
    /// </summary>
    public virtual void Load() { }

    public virtual void Open() {
        TryOpen();
    }
    
    public virtual void Close() {
        IsOpen = false;
    }

    public void UnCollapseOrShow() {
        TryUnCollapse();
        TryOpen();
    }

    public void UnCollapseOrToggle() {
        TryUnCollapse();
        Toggle();
    }

    private void TryUnCollapse() {
        if (isCollapsed) {
            UnCollapse();
        }
    }

    private void UnCollapse() {
        isCollapsed = false;
        Collapsed = false;
    }

    public override void Update() {
        isCollapsed = true;
    }

    public override void Draw() {
        isCollapsed = false;
        Collapsed = null;
    }

    public new void Toggle() {
        if (IsOpen) {
            IsOpen = false;
        }
        else {
            TryOpen();
        }
    }

    private void TryOpen() {
        if (IsOpenAllowed()) {
            IsOpen = true;
        }
        else {
            PrintOpenNotAllowed();
        }
    }
}
