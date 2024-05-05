using System.Collections.Generic;
using Dalamud.Interface.Utility.Raii;

namespace KamiLib.TabBar;

public interface ITabItem {
    string Name { get; }
    bool Disabled { get; }
    void Draw();
}

public class TabBar(string name, List<ITabItem> tabs) {
    public void Draw() {
        using var tabBar = ImRaii.TabBar(name);
        foreach (var tab in tabs) {
            using var disabled = ImRaii.Disabled(tab.Disabled);
            using var tabItem = ImRaii.TabItem(tab.Name);
            
            if (tabItem) {
                using var child = ImRaii.Child($"tab_{tab.Name}_child");
                tab.Draw();
            }
        }
    }
}