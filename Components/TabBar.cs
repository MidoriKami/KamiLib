using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace KamiLib.Components;

public interface ITabItem {
    string Name { get; }
    bool Disabled { get; }
    void Draw();
}

public class TabBar(string name, List<ITabItem> tabs, bool allowScrolling = true) {
    public void Draw() {
        using var tabBar = ImRaii.TabBar(name);
        foreach (var tab in tabs) {
            using var disabled = ImRaii.Disabled(tab.Disabled);
            using var tabItem = ImRaii.TabItem(tab.Name);
            
            if (tabItem) {
                using var child = ImRaii.Child($"tab_{tab.Name}_child", new Vector2(0.0f, 0.0f), false, allowScrolling ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
                tab.Draw();
            }
        }
    }
}