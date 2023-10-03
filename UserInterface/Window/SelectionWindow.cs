using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.Interfaces;

namespace KamiLib.UserInterface;

public abstract class SelectionWindow : Window, IDrawable
{
    private const bool ShowBorders = false;
    private readonly float initialSelectionWidth;

    private readonly SelectionList selectionList = new();
    private readonly float verticalHeight;
    protected bool ShowScrollBar = true;
    
    internal ISelectable? CurrentSelection
    {
        get => selectionList.Selected;
        set => selectionList.Selected = value;
    }

    protected SelectionWindow(string windowName, float height = 0.0f, float initialSelectionWidth = 250.0f) : base(windowName)
    {
        verticalHeight = height;
        this.initialSelectionWidth = initialSelectionWidth;

        Flags |= ImGuiWindowFlags.NoScrollbar;
        Flags |= ImGuiWindowFlags.NoScrollWithMouse;
    }

    public override void Draw()
    {
        var region = ImGui.GetContentRegionAvail();
        var itemSpacing = ImGui.GetStyle().ItemSpacing;

        var topLeftSideHeight = region.Y - verticalHeight * ImGuiHelpers.GlobalScale - itemSpacing.Y / 2.0f;

        if (ImGui.BeginTable($"{KamiCommon.PluginName}TableContainer", 2, ImGuiTableFlags.Resizable))
        {
            ImGui.TableSetupColumn("##LeftColumn", ImGuiTableColumnFlags.WidthFixed, initialSelectionWidth);

            ImGui.TableNextColumn();

            var regionSize = ImGui.GetContentRegionAvail();

            if (ImGui.BeginChild($"###{KamiCommon.PluginName}LeftSide", regionSize with { Y = topLeftSideHeight }, ShowBorders, ImGuiWindowFlags.NoDecoration))
            {
                selectionList.Draw(GetSelectables());
            }
            ImGui.EndChild();

            if (ImGui.BeginChild($"###{KamiCommon.PluginName}BottomLeftSide", regionSize with { Y = verticalHeight * ImGuiHelpers.GlobalScale }, ShowBorders, ImGuiWindowFlags.NoDecoration))
            {
                DrawExtras();
            }
            ImGui.EndChild();

            ImGui.TableNextColumn();
            if (ImGui.BeginChild($"###{KamiCommon.PluginName}RightSide", Vector2.Zero, ShowBorders, (ShowScrollBar ? ImGuiWindowFlags.AlwaysVerticalScrollbar : ImGuiWindowFlags.None) | ImGuiWindowFlags.NoDecoration))
            {
                selectionList.DrawSelected();
            }
            ImGui.EndChild();

            ImGui.EndTable();
        }

        DrawSpecial();
    }

    protected abstract IEnumerable<ISelectable> GetSelectables();
    
    protected virtual void DrawExtras()
    {
    }
    
    protected virtual void DrawSpecial()
    {
    }
}