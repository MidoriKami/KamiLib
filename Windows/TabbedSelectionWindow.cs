using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using KamiLib.Interfaces;

namespace KamiLib.Windows;

public abstract class TabbedSelectionWindow : SelectionWindow
{
    protected TabbedSelectionWindow(string windowName, float height = 0, float initialSelectionWidth = 250.0f) : base(windowName, height, initialSelectionWidth) { }
    
    private ISelectionWindowTab? selectedTab;

    protected abstract IEnumerable<ISelectionWindowTab> GetTabs();
    protected virtual IEnumerable<ITabItem> GetRegularTabs() => new List<ITabItem>();

    protected bool Reorderable { get; set; } = true;
    private bool suppressSelectionSystem;

    public override void Draw()
    {
        suppressSelectionSystem = false;
        selectedTab ??= GetTabs().First();
        if (ImGui.BeginTabBar("TabBar", Reorderable ? ImGuiTabBarFlags.Reorderable : ImGuiTabBarFlags.None))
        {
            foreach (var tab in GetTabs())
            {
                if (ImGui.BeginTabItem(tab.TabName))
                {
                    if (selectedTab.TabName != tab.TabName)
                    {
                        selectedTab.LastSelection = CurrentSelection;
                        selectedTab = tab;
                        CurrentSelection = tab.LastSelection;
                    }
                    ImGui.EndTabItem();
                }
            }

            foreach (var tab in GetRegularTabs())
            {
                if (!tab.Enabled) continue;

                if (ImGui.BeginTabItem(tab.TabName))
                {
                    if (ImGui.BeginChild("##TabChild", -Vector2.One, false, ImGuiWindowFlags.AlwaysVerticalScrollbar))
                    {
                        tab.Draw();
                    }
                    ImGui.EndChild();
                    
                    suppressSelectionSystem = true;
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
        
        if(!suppressSelectionSystem) base.Draw();
    }

    protected override IEnumerable<ISelectable> GetSelectables()
    {
        return selectedTab?.GetTabSelectables() ?? ImmutableList<ISelectable>.Empty;
    }

    protected virtual void DrawWindowExtras()
    {
        
    }

    protected override void DrawExtras()
    {
        var region = ImGui.GetContentRegionAvail();
        var padding = ImGui.GetStyle().FramePadding;
        
        if (ImGui.BeginChild("###TabbedWindowTabExtras", region with { Y = region.Y / 2.0f - padding.Y}))
        {
            selectedTab?.DrawTabExtras();
        }
        ImGui.EndChild();

        if (ImGui.BeginChild("###TabbedWindowWindowExtras", region with { Y = region.Y / 2.0f - padding.Y}))
        {
            DrawWindowExtras();
        }
        ImGui.EndChild();
    }
}
