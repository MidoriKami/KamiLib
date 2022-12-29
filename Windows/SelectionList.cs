using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;
using KamiLib.Interfaces;

namespace KamiLib.Windows;

public class SelectionList
{
    public ISelectable? Selected { get; private set; }
    
    public void Draw(IEnumerable<ISelectable> selectables)
    {
        var frameBgColor = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];
        ImGui.PushStyleColor(ImGuiCol.FrameBg, frameBgColor with { W = 0.05f });

        ImGui.PushStyleVar(ImGuiStyleVar.ScrollbarSize, 0.0f);
        var listBoxValid = ImGui.BeginListBox("", new Vector2(-1, -1));
        ImGui.PopStyleVar();

        if (listBoxValid)
        {
            foreach (var item in selectables)
            {
                var headerHoveredColor = ImGui.GetStyle().Colors[(int)ImGuiCol.HeaderHovered];
                var textSelectedColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Header];
                ImGui.PushStyleColor(ImGuiCol.HeaderHovered, headerHoveredColor with { W = 0.1f });
                ImGui.PushStyleColor(ImGuiCol.Header, textSelectedColor with { W = 0.1f });

                if (ImGui.Selectable("", Selected == item))
                {
                    Selected = Selected == item ? null : item;
                }

                ImGui.PopStyleColor();
                ImGui.PopStyleColor();

                ImGui.SameLine(3.0f);

                item.DrawLabel();

                ImGui.Spacing();
            }

            ImGui.EndListBox();
        }
    }

    public void DrawSelected()
    {
        if (Selected is null)
        {
            const string text = "Select an item in the left pane";
            var available = ImGui.GetContentRegionAvail() / 2.0f;
            var textSize = ImGui.CalcTextSize(text) / 2.0f;
            var center = new Vector2(available.X - textSize.X, available.Y - textSize.Y);

            ImGui.SetCursorPos(center);
            ImGui.TextWrapped(text);
        }
        else
        {
            Selected.Contents.Draw();
        }
    }
}