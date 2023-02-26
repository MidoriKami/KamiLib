using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ImGuiNET;
using KamiLib.Interfaces;

namespace KamiLib.Windows;

public abstract class TabbedSelectionWindow : SelectionWindow
{
    protected TabbedSelectionWindow(string windowName, float height = 0, float initialSelectionWidth = 250) : base(windowName, height, initialSelectionWidth) { }
    
    private ISelectionWindowTab? selectedTab;

    protected abstract IEnumerable<ISelectionWindowTab> GetTabs();


    public override void Draw()
    {
        selectedTab ??= GetTabs().First();
        if (ImGui.BeginTabBar("TabBar"))
        {
            foreach (var tab in GetTabs())
            {
                if (ImGui.BeginTabItem(tab.TabName))
                {
                    if (selectedTab.Id != tab.Id)
                    {
                        selectedTab.LastSelection = CurrentSelection;
                        selectedTab = tab;
                        CurrentSelection = tab.LastSelection;
                    }
                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
        base.Draw();
    }

    protected override IEnumerable<ISelectable> GetSelectables()
    {
        return selectedTab?.GetTabSelectables() ?? ImmutableList<ISelectable>.Empty;
    }

    protected abstract void DrawWindowExtras();

    protected override void DrawExtras()
    {
        selectedTab?.DrawTabExtras();
        DrawWindowExtras();
    }
}
