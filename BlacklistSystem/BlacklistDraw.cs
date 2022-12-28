using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Logging;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Configuration;
using KamiLib.Extensions;
using KamiLib.InfoBoxSystem;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.BlacklistSystem;

public static class BlacklistDraw
{
    private static readonly List<uint> EntriesToRemove = new();
    private static readonly List<uint> EntriesToAdd = new();
    private static string _searchString = "Search . . . ";
    private static List<SearchResult>? _searchResults = new();
    
    public static void DrawBlacklist(Setting<List<uint>> blacklistedAreas)
    {
        InfoBox.Instance
            .AddTitle("Currently Blacklisted Areas", out var innerWidth, 1.0f)
            .AddDummy(5.0f)
            .AddAction(() => BlacklistedAreasList(blacklistedAreas))
            .AddDisabledButton(EntriesToRemove.Count == 0 ? "Clear Blacklist" : $"Remove {EntriesToRemove.Count} Selected Areas", () =>
            {
                if (EntriesToRemove.Count == 0)
                {
                    blacklistedAreas.Value.Clear();
                    KamiLib.SaveConfiguration();
                }
                else
                {
                    blacklistedAreas.Value.RemoveAll(entry => EntriesToRemove.Contains(entry));
                    EntriesToRemove.Clear();
                    KamiLib.SaveConfiguration();
                }
            }, !ImGui.GetIO().KeyShift, "Hold 'Shift' to enable button", innerWidth)
            .Draw();
    }

    public static void DrawAddRemoveHere(Setting<List<uint>> blacklistedZones)
    {
        InfoBox.Instance
            .AddTitle("Add or Remove Current Zone", 1.0f)
            .AddAction(() => TerritoryTypeCache.Instance.GetRow(Service.ClientState.TerritoryType)?.DrawLabel())
            .BeginTable()
            .BeginRow()
            .AddDisabledButton("Add", () =>
                { 
                    Add(blacklistedZones, Service.ClientState.TerritoryType);
                }, blacklistedZones.Value.Contains(Service.ClientState.TerritoryType), buttonSize: InfoBox.Instance.InnerWidth / 2.0f - 5.0f * ImGuiHelpers.GlobalScale)
            .AddDisabledButton("Remove", () =>
                {
                    Remove(blacklistedZones, Service.ClientState.TerritoryType);
                }, !blacklistedZones.Value.Contains(Service.ClientState.TerritoryType), buttonSize: InfoBox.Instance.InnerWidth / 2.0f - 5.0f * ImGuiHelpers.GlobalScale)
            .EndRow()
            .EndTable()
            .Draw();
    }

    public static void DrawTerritorySearch(Setting<List<uint>> blacklistedZones)
    {
        InfoBox.Instance
            .AddTitle("Zone Search", out var innerWidth, 1.0f)
            .AddAction(() =>
            {
                ImGui.PushItemWidth(InfoBox.Instance.InnerWidth);
                if (ImGui.InputText("###TerritorySearch", ref _searchString, 60, ImGuiInputTextFlags.AutoSelectAll))
                {
                    _searchResults = Search(_searchString, 5);
                    PluginLog.Debug("Updating TerritorySearch Results");
                }
            })
            .AddAction(() => DisplayResults(_searchResults))
            .AddDisabledButton($"Add {EntriesToAdd.Count} Selected Areas", () =>
            {
                blacklistedZones.Value.AddRange(EntriesToAdd);
                EntriesToAdd.Clear();
                KamiLib.SaveConfiguration();
                
            }, !EntriesToAdd.Any(), "Select zones to add to blacklist", innerWidth)
            .Draw();
    }

    public static void PrimeSearch()
    {
        _searchResults = Search("", 5);
    }
    
    private static List<SearchResult> Search(string searchTerms, int numResults)
    {
        return Service.DataManager.GetExcelSheet<TerritoryType>()!
            .Where(territory => territory.PlaceName.Row is not 0)
            .Where(territory => territory.PlaceName.Value is not null)
            .GroupBy(territory => territory.PlaceName.Value!.Name.ToDalamudString().TextValue)
            .Select(territory => territory.First())
            .Where(territory => territory.PlaceName.Value!.Name.ToDalamudString().TextValue.ToLower().Contains(searchTerms.ToLower()))
            .Select(territory => new SearchResult {
                TerritoryID = territory.RowId
            })
            .OrderBy(searchResult => searchResult.TerritoryName)
            .Take(numResults)
            .ToList();
    }

    private static void DisplayResults(List<SearchResult>? results)
    {
        if (results is null) return; 
        
        if (ImGui.BeginChild("###SearchResultsChild", new Vector2(InfoBox.Instance.InnerWidth, 21.0f * 5 * ImGuiHelpers.GlobalScale )))
        {
            foreach (var result in results)
            {
                ImGui.SetNextItemWidth(InfoBox.Instance.InnerWidth);
                if (ImGui.Selectable($"###SearchResult{result.TerritoryID}", EntriesToAdd.Contains(result.TerritoryID)))
                {
                    if (!EntriesToAdd.Contains(result.TerritoryID))
                    {
                        EntriesToAdd.Add(result.TerritoryID);
                    }
                    else
                    {
                        EntriesToAdd.Remove(result.TerritoryID);
                    }
                }
                    
                ImGui.SameLine();
                var startPosition = ImGui.GetCursorPos();
                ImGui.TextColored(Colors.Grey, result.TerritoryID.ToString());
                ImGui.SameLine(startPosition.X + 50.0f * ImGuiHelpers.GlobalScale);
                ImGui.Text(result.TerritoryName);
            }
        }
        ImGui.EndChild();
    }

    private static void BlacklistedAreasList(Setting<List<uint>> blacklistedAreas)
    {
        var itemCount = Math.Min(blacklistedAreas.Value.Count, 10);
        var listHeight = itemCount * ImGuiHelpers.GlobalScale * 21.0f;
        var minHeight = 21.0f * ImGuiHelpers.GlobalScale;

        var size = new Vector2(InfoBox.Instance.InnerWidth, MathF.Max(listHeight, minHeight));
        
        if(ImGui.BeginChild("###BlacklistFrame", size, false))
        {
            if (!blacklistedAreas.Value.Any())
            {
                ImGui.TextColored(Colors.Orange, "List is Empty");
            }
            else
            {
                DrawBlacklistedAreas(blacklistedAreas);
            }
        }
        ImGui.EndChild();
    }

    private static void DrawBlacklistedAreas(Setting<List<uint>> blacklistedAreas)
    {
        foreach (var territory in blacklistedAreas.Value)
        {
            var territoryType = TerritoryTypeCache.Instance.GetRow(territory);

            ImGui.PushItemWidth(InfoBox.Instance.InnerWidth);
            if (ImGui.Selectable($"###{territory}", EntriesToRemove.Contains(territory)))
            {
                if (!EntriesToRemove.Contains(territory))
                {
                    EntriesToRemove.Add(territory);
                }
                else
                {
                    EntriesToRemove.Remove(territory);
                }
            }
            
            ImGui.SameLine();
            territoryType?.DrawLabel();
        }
    }
    
    private static void Add(Setting<List<uint>> zones, uint id)
    {
        if (!zones.Value.Contains(id))
        {
            zones.Value.Add(id);
            KamiLib.SaveConfiguration();
        }
    }

    private static void Remove(Setting<List<uint>> zones, uint id)
    {
        if (zones.Value.Contains(id))
        {
            zones.Value.Remove(id);
            KamiLib.SaveConfiguration();
        }
    }
}