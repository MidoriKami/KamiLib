using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Action = System.Action;

namespace KamiLib.Classes;

public interface ITabItem {
    string Name { get; }
    bool Disabled { get; }
    void Draw();
}

public class SimpleTabItem(string label, Action drawAction) : ITabItem {
    public string Name => label;
    public bool Disabled => false;
    
    public void Draw()
        => drawAction();
}

public class TabBar(string name, List<ITabItem> tabs, bool allowScrolling = true) {
    public void Draw() {
        using var tabBar = ImRaii.TabBar(name);
        if (!tabBar) return;

        foreach (var tab in tabs) {
            using var disabled = ImRaii.Disabled(tab.Disabled);
            DrawTabItem(tab);
        }
    }

    private void DrawTabItem(ITabItem tab) {
        using var tabItem = ImRaii.TabItem(tab.Name);
        if (!tabItem) return;

        DrawTabChild(tab);
    }

    private void DrawTabChild(ITabItem tab) {
        using var child = ImRaii.Child($"tab_{tab.Name}_child", new Vector2(0.0f, 0.0f), false, allowScrolling ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
        if (!child) return;

        tab.Draw();
    }
}
