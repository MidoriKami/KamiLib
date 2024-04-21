using System;
using System.Numerics;

namespace KamiLib.Window;

public abstract class Window : Dalamud.Interface.Windowing.Window {
    private bool isCollapsed;

    protected Window(string windowName, Vector2 size, bool fixedSize = false) : base(windowName) {
        SizeConstraints = new WindowSizeConstraints {
            MinimumSize = size,
            MaximumSize = fixedSize ? size : new Vector2(float.PositiveInfinity),
        };
    }
    
    public abstract void PrintOpenNotAllowed();

    public abstract bool IsOpenAllowed();

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
