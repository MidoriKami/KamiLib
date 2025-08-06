using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using KamiLib.Classes;

namespace KamiLib.Window;

public abstract class TabbedSelectionWindow<T>(string windowName, Vector2 size, bool fixedSize = false) : SelectionListWindow<T>(windowName, size, fixedSize) where T : notnull {
    protected abstract string SelectionListTabName { get; }

    protected abstract List<ITabItem> Tabs { get; }

    protected virtual bool AllowTabScrollBar => true;
    protected virtual bool AllowTabScroll => true;
    
    private string selectedTab = string.Empty;
    
    protected override void DrawContents() {
        using var windowTabBar = ImRaii.TabBar("tabbedSelectionTabBar", ImGuiTabBarFlags.Reorderable);
        if (!windowTabBar) return;

        using (var selectionListTab = ImRaii.TabItem(SelectionListTabName)) {
            if (selectionListTab) {
                if (selectedTab != SelectionListTabName) {
                    OnTabChanged();
                    selectedTab = SelectionListTabName;
                }
                
                base.DrawContents();
            }
        }
        
        foreach (var tab in Tabs) {
            using var disabled = ImRaii.Disabled(tab.Disabled);
            using var tabItem = ImRaii.TabItem(tab.Name);
            
            if (tabItem) {
                if (selectedTab != tab.Name) {
                    OnTabChanged();
                    selectedTab = tab.Name;
                }
                
                var flags = ImGuiWindowFlags.None;
                flags |= AllowTabScrollBar ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoScrollbar;
                flags |= AllowTabScroll ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoScrollWithMouse;
                
                using var child = ImRaii.Child($"tab_{tab.Name}_child", new Vector2(0.0f, 0.0f), false, flags);
                tab.Draw();
            }
        }
    }
    
    public virtual void OnTabChanged() { }
}