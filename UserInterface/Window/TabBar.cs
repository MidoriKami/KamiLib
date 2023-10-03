using System.Collections.Generic;
using ImGuiNET;
using KamiLib.Interfaces;

namespace KamiLib.UserInterface;

public class TabBar
{
    public IEnumerable<ITabItem>? TabItems { get; set; }
    public string Id { get; set; } = "UnknownId";

    public void Draw()
    {
        if (TabItems is null) return;

        if (ImGui.BeginTabBar($"##TabBar{Id}"))
        {
            foreach (var tab in TabItems)
            {
                if (!tab.Enabled) continue;

                if (ImGui.BeginTabItem($"{tab.TabName}"))
                {
                    if (ImGui.BeginChild($"##TabChild{tab.TabName}{Id}"))
                    {
                        tab.Draw();
                    }
                    ImGui.EndChild();

                    ImGui.EndTabItem();
                }
            }

            ImGui.EndTabBar();
        }
    }
}