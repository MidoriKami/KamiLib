using System.Collections.Generic;
using Dalamud.Interface.Utility.Raii;

namespace KamiLib.TabBar;

public class TabBar(string name, List<ITabItem> tabs) {
    public void Draw() {
        using var tabBar = ImRaii.TabBar(name);
        foreach (var tab in tabs) {
            using var disabled = ImRaii.Disabled(!tab.Enabled);
            using var tabItem = ImRaii.TabItem(tab.Name);
            
            tab.Draw();
        }
    }
}