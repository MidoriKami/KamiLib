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
using KamiLib.Drawing;
using KamiLib.Extensions;
using KamiLib.Localization;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.ZoneFilterList;

public static class ZoneFilterListDraw
{
    private static readonly List<uint> EntriesToRemove = new();
    private static readonly List<uint> EntriesToAdd = new();
    private static string _searchString = string.Empty;
    private static List<SearchResult>? _searchResults = new();

    public static void DrawFilterTypeRadio(Setting<ZoneFilterTypeId> currentType)
    {
        PluginLog.LogDebug(currentType.Value.ToString());
        InfoBox.Instance
               .AddTitle(Strings.ZoneFilterList_Type_Section, percentFill: 1.0f)
               .AddConfigRadio(Strings.ZoneFilterList_Type_AllowLabel, currentType, ZoneFilterType.WhiteList.Id, Strings.ZoneFilterList_Type_AllowTooltip)
               .SameLine()
               .AddConfigRadio(Strings.ZoneFilterList_Type_BlockLabel, currentType, ZoneFilterType.BlackList.Id, Strings.ZoneFilterList_Type_BlockTooltip)
               .Draw();
    }
    
    public static void DrawZoneList(Setting<List<uint>> currentZones, ZoneFilterType zoneFilterType)
    {
        InfoBox.Instance
            .AddTitle(zoneFilterType.CurrentLabel, out var innerWidth, 1.0f)
            .AddDummy(5.0f)
            .AddAction(() => SelectedZonesList(currentZones, zoneFilterType))
            .AddDisabledButton(EntriesToRemove.Count == 0 ? zoneFilterType.ClearButton : Strings.ZoneFilterList_RemoveSelectedZones.Format(EntriesToRemove.Count), () =>
            {
                if (EntriesToRemove.Count == 0)
                {
                    currentZones.Value.Clear();
                    KamiCommon.SaveConfiguration();
                }
                else
                {
                    currentZones.Value.RemoveAll(entry => EntriesToRemove.Contains(entry));
                    EntriesToRemove.Clear();
                    KamiCommon.SaveConfiguration();
                }
            }, !ImGui.GetIO().KeyShift, Strings.DisabledButton_HoldShift, innerWidth)
            .Draw();
    }

    public static void DrawAddRemoveHere(Setting<List<uint>> selectedZones)
    {
        InfoBox.Instance
            .AddTitle(Strings.ZoneFilterList_AddRemoveZone, 1.0f)
            .AddAction(() => LuminaCache<TerritoryType>.Instance.GetRow(Service.ClientState.TerritoryType)?.DrawLabel())
            .BeginTable()
            .BeginRow()
            .AddDisabledButton(Strings.Common_Add, () =>
                { 
                    Add(selectedZones, Service.ClientState.TerritoryType);
                }, selectedZones.Value.Contains(Service.ClientState.TerritoryType), buttonSize: InfoBox.Instance.InnerWidth / 2.0f - 5.0f * ImGuiHelpers.GlobalScale)
            .AddDisabledButton(Strings.Common_Remove, () =>
                {
                    Remove(selectedZones, Service.ClientState.TerritoryType);
                }, !selectedZones.Value.Contains(Service.ClientState.TerritoryType), buttonSize: InfoBox.Instance.InnerWidth / 2.0f - 5.0f * ImGuiHelpers.GlobalScale)
            .EndRow()
            .EndTable()
            .Draw();
    }

    public static void DrawTerritorySearch(Setting<List<uint>> selectedZones, ZoneFilterType zoneFilterType)
    {
        InfoBox.Instance
            .AddTitle(Strings.ZoneFilterList_ZoneSearch, out var innerWidth, 1.0f)
            .AddAction(() =>
            {
                ImGui.PushItemWidth(InfoBox.Instance.InnerWidth);
                if (ImGui.InputTextWithHint("###TerritorySearch", Strings.ZoneFilterList_Search, ref _searchString, 60, ImGuiInputTextFlags.AutoSelectAll))
                {
                    _searchResults = Search(_searchString, 5);
                    PluginLog.Debug("Updating TerritorySearch Results");
                }
            })
            .AddAction(() => DisplayResults(_searchResults))
            .AddDisabledButton(Strings.ZoneFilterList_AddSelectedZones.Format(EntriesToAdd.Count), () =>
            {
                selectedZones.Value.AddRange(EntriesToAdd);
                EntriesToAdd.Clear();
                KamiCommon.SaveConfiguration();
                
            }, !EntriesToAdd.Any(), zoneFilterType.SelectZonesLabel, innerWidth)
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
                LuminaCache<TerritoryType>.Instance.GetRow(result.TerritoryID)?.DrawLabel();
            }
        }
        ImGui.EndChild();
    }

    private static void SelectedZonesList(Setting<List<uint>> selectedZones, ZoneFilterType zoneFilterType)
    {
        var itemCount = Math.Min(selectedZones.Value.Count, 10);
        var listHeight = itemCount * ImGuiHelpers.GlobalScale * 21.0f;
        var minHeight = 21.0f * ImGuiHelpers.GlobalScale;

        var size = new Vector2(InfoBox.Instance.InnerWidth, MathF.Max(listHeight, minHeight));
        
        if(ImGui.BeginChild("###ZoneFilterListFrame", size, false))
        {
            if (!selectedZones.Value.Any())
            {
                ImGui.SetCursorPos(ImGui.GetCursorPos() with { X = ImGui.GetContentRegionAvail().X / 2 - ImGui.CalcTextSize(zoneFilterType.IsEmptyLabel).X / 2.0f});
                ImGui.TextColored(Colors.Orange, zoneFilterType.IsEmptyLabel);
            }
            else
            {
                DrawSelectedZones(selectedZones);
            }
        }
        ImGui.EndChild();
    }

    private static void DrawSelectedZones(Setting<List<uint>> selectedZones)
    {
        var territories = selectedZones.Value
            .Select(zone => LuminaCache<TerritoryType>.Instance.GetRow(zone))
            .OfType<TerritoryType>()
            .OrderBy(territory => territory.GetPlaceNameString());
        
        foreach (var territory in territories)
        {
            ImGui.PushItemWidth(InfoBox.Instance.InnerWidth);
            if (ImGui.Selectable($"###{territory}", EntriesToRemove.Contains(territory.RowId)))
            {
                if (!EntriesToRemove.Contains(territory.RowId))
                {
                    EntriesToRemove.Add(territory.RowId);
                }
                else
                {
                    EntriesToRemove.Remove(territory.RowId);
                }
            }
            
            ImGui.SameLine();
            territory.DrawLabel();
        }
    }
    
    private static void Add(Setting<List<uint>> zones, uint id)
    {
        if (!zones.Value.Contains(id))
        {
            zones.Value.Add(id);
            KamiCommon.SaveConfiguration();
        }
    }

    private static void Remove(Setting<List<uint>> zones, uint id)
    {
        if (zones.Value.Contains(id))
        {
            zones.Value.Remove(id);
            KamiCommon.SaveConfiguration();
        }
    }
}
