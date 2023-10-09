using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Game;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Search;

public class TerritorySearchModal
{
    private readonly List<TerritoryType> selectedTerritories = new();
    private List<TerritoryType> searchResults = new();
    private string searchString = string.Empty;

    public required string Name { get; init; }
    public required Action<TerritoryType> AddAction { get; init; }
    
    public void DrawSearchModal()
    {
        ImGui.SetNextWindowSizeConstraints(new Vector2(500.0f, 300.0f), new Vector2(500.0f, 300.0f));
        if (ImGui.BeginPopupModal(Name))
        {
            if (ImGui.IsWindowAppearing())
            {
                ImGui.SetKeyboardFocusHere();
            }

            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            if (ImGui.InputTextWithHint("##SearchBox", "Search...", ref searchString, 2048, ImGuiInputTextFlags.AutoSelectAll))
            {
                UpdateSearch();
            }
            ImGuiHelpers.ScaledDummy(5.0f);

            var childSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 23.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().FramePadding.Y - 5.0f * ImGuiHelpers.GlobalScale);
            if (ImGui.BeginChild("##SearchResults", childSize))
            {
                ImGuiHelpers.ScaledDummy(2.0f);

                if (searchResults is { Count: > 0 })
                {
                    ImGuiClip.ClippedDraw(searchResults, DrawSearchResult, ImGui.GetTextLineHeightWithSpacing());
                }
                else
                {
                    const string text = "No Results";
                    var textSize = ImGui.CalcTextSize(text);
                    ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X / 2.0f - textSize.X / 2.0f);
                    ImGui.SetCursorPosY(ImGui.GetContentRegionAvail().Y / 2.0f - textSize.Y / 2.0f);
                    ImGui.TextColored(KnownColor.OrangeRed.Vector(), text);
                }
            }
            ImGui.EndChild();
            DrawAddCancelButtons();

            ImGui.EndPopup();
        }
    }
    
    private void DrawSearchResult(TerritoryType territoryType)
    {
        if (ImGui.Selectable($"##SearchResult{territoryType.RowId}", selectedTerritories.Contains(territoryType)))
        {
            if (!selectedTerritories.Contains(territoryType))
            {
                selectedTerritories.Add(territoryType);
            }
            else
            {
                selectedTerritories.Remove(territoryType);
            }
        }
        
        ImGui.SameLine();
        ImGui.TextColored(KnownColor.Gray.Vector(), territoryType.RowId.ToString());
        
        ImGui.SameLine(50.0f);
        ImGui.TextUnformatted(territoryType.PlaceName.Value?.Name.ToDalamudString().ToString());

        ImGui.SameLine();
        if (GetDutyNameForTerritoryType(territoryType.RowId) is { } dutyName && !dutyName.IsNullOrEmpty())
        {
            ImGui.TextColored(KnownColor.Gray.Vector(), $"( {dutyName} )");
        }
        else
        {
            ImGui.Text(string.Empty);
        }
    }

    private void DrawAddCancelButtons()
    {
        ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - 23.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Cancel", ImGuiHelpers.ScaledVector2(100.0f, 23.0f)))
        {
            selectedTerritories.Clear();
            ImGui.CloseCurrentPopup();
        }

        ImGui.SameLine(ImGui.GetContentRegionMax().X - 100.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Add", ImGuiHelpers.ScaledVector2(100.0f, 23.0f)))
        {
            if (selectedTerritories.Any())
            {
                foreach (var selected in selectedTerritories)
                {
                    AddAction(selected);
                }
                
                selectedTerritories.Clear();
            }
            else
            {
                Service.Chat.PrintError("No item selected");
            }
            ImGui.CloseCurrentPopup();
        }
    }
    
    private void UpdateSearch()
    {
        searchResults = Service.DataManager.GetExcelSheet<TerritoryType>()!
            .Where(territory => territory is { RowId: not 0, PlaceName.Value.RowId: not 0, QuestBattle.Row: 0, TerritoryIntendedUse: not 15 and not 29 })
            .Where(territory => territory.PlaceName.Value!.Name.ToDalamudString().TextValue.ToLowerInvariant().Contains(searchString.ToLowerInvariant()))
            .OrderBy(searchResult => searchResult.RowId)
            .Prepend(LuminaCache<TerritoryType>.Instance.GetRow(Service.ClientState.TerritoryType)!)
            .ToList();
    }
    
    private static string? GetDutyNameForTerritoryType(uint territory)
    {
        if (LuminaCache<TerritoryType>.Instance.GetRow(territory) is not { ContentFinderCondition.Row: var cfcRow }) return null;
        if (LuminaCache<ContentFinderCondition>.Instance.GetRow(cfcRow) is not { Name: var dutyName }) return null;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dutyName.ToDalamudString().TextValue);
    }
}