using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib;
using KamiLib.AutomaticUserInterface;
using KamiLib.Caching;
using KamiLib.Localization;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;
using Action = System.Action;

namespace NoTankYou.Models.Attributes;

public class BlacklistAttribute : DrawableAttribute
{
    private class SearchResult
    {
        public uint TerritoryID { get; init; }
        private uint PlaceNameRow => LuminaCache<TerritoryType>.Instance.GetRow(TerritoryID)?.PlaceName.Row ?? 0;
        public string TerritoryName => LuminaCache<PlaceName>.Instance.GetRow(PlaceNameRow)?.Name.ToDalamudString().TextValue ?? "Unknown PlaceName Row";
    }
    
    private static string _searchString = string.Empty;
    private static List<SearchResult>? _searchResults = new();

    public BlacklistAttribute() : base(null)
    {
        _searchResults = Search("", 5);
    }
    
    protected override void Draw(object obj, MemberInfo field, Action? saveAction = null)
    {
        ImGui.Indent(-15.0f);
        
        var hashSet = GetValue<HashSet<uint>>(obj, field);
        
        DrawSearchBox();
        DrawSearchResults(obj, field, saveAction, hashSet);
        DrawCurrentlyBlacklisted(obj, field, saveAction, hashSet);
        
        ImGui.Indent(15.0f);
    }
    
    private static void DrawSearchBox()
    {
        ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputTextWithHint("##SearchBox", Strings.Search, ref _searchString, 64, ImGuiInputTextFlags.AutoSelectAll))
        {
            _searchResults = Search(_searchString, 5);
        }
    }

    private void DrawSearchResults(object obj, MemberInfo field, Action? saveAction, IReadOnlySet<uint> hashSet)
    {
        if (ImGui.BeginChild("##SearchResults", ImGui.GetContentRegionAvail() with { Y = 325.0f * ImGuiHelpers.GlobalScale }, false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            if (_searchResults is null) return;
            
            ImGui.TextUnformatted("Current Area");
            ImGui.Separator();
            DrawConfigurableTerritoryLine("CurrentArea", obj, field, saveAction, hashSet, Service.ClientState.TerritoryType);

            ImGui.TextUnformatted("Search Results");
            ImGui.Separator();
            foreach (var result in _searchResults) DrawConfigurableTerritoryLine("SearchResult", obj, field, saveAction, hashSet, result.TerritoryID);
        }
        ImGui.EndChild();
        
        ImGuiHelpers.ScaledDummy(5.0f);
    }
    
    private void DrawCurrentlyBlacklisted(object obj, MemberInfo field, Action? saveAction, IReadOnlyCollection<uint> hashSet)
    {
        ImGui.TextUnformatted("Currently Blacklisted");
        ImGui.Separator();

        if (hashSet.Count is 0) ImGui.TextUnformatted(Strings.NothingBlacklisted);
        uint? removalZone = null;

        foreach (var zone in hashSet)
        {
            if (ImGuiComponents.IconButton($"RemoveButton{zone}", FontAwesomeIcon.Trash)) removalZone = zone;
            ImGui.SameLine();
            DrawTerritory(zone);
        }

        if(removalZone is { } toRemove) RemoveZone(obj, field, saveAction, toRemove);
    }
    
    private void DrawConfigurableTerritoryLine(string additionalId, object obj, MemberInfo field, Action? saveAction, IReadOnlySet<uint> hashSet, uint territoryId)
    {
        DrawAddRemoveButton(additionalId, obj, field, saveAction, hashSet, territoryId);
        ImGui.SameLine();
        DrawTerritory(territoryId);
    }

    private void DrawAddRemoveButton(string additionalId, object obj, MemberInfo field, Action? saveAction, IReadOnlySet<uint> hashSet, uint territoryId)
    {
        if (hashSet.Contains(territoryId))
        {
            if (ImGuiComponents.IconButton($"RemoveButton{territoryId}{additionalId}", FontAwesomeIcon.Trash))
            {
                RemoveZone(obj, field, saveAction, territoryId);
            }
        }
        else
        {
            if (ImGuiComponents.IconButton($"AddButton{territoryId}{additionalId}", FontAwesomeIcon.Plus))
            {
                AddZone(obj, field, saveAction, territoryId);
            }
        }
    }

    private void AddZone(object obj, MemberInfo field, Action? saveAction, uint zone)
    {
        var hashSet = GetValue<HashSet<uint>>(obj, field);
        hashSet.Add(zone);
        saveAction?.Invoke();
    }
    
    private void RemoveZone(object obj, MemberInfo field, Action? saveAction, uint zone)
    {
        var hashSet = GetValue<HashSet<uint>>(obj, field);
        hashSet.Remove(zone);
        saveAction?.Invoke();
    }
    
    private void DrawTerritory(uint zoneId)
    {
        var zone = LuminaCache<TerritoryType>.Instance.GetRow(zoneId);
        if (zone is null) return;

        var placeNameKey = zone.PlaceName.Row;
        var placeNameStringRow = LuminaCache<PlaceName>.Instance.GetRow(placeNameKey)!;
        var territoryName = placeNameStringRow.Name.ToDalamudString().ToString();
        
        if (ImGui.BeginTable($"##TerritoryTypeTable{zone.RowId}", 2))
        {
            ImGui.TableSetupColumn($"##TerritoryRow{zone.RowId}", ImGuiTableColumnFlags.WidthFixed, 50.0f);
            ImGui.TableSetupColumn($"##Label{zone.RowId}");

            ImGui.TableNextColumn();
            ImGui.TextColored(KnownColor.Gray.Vector(), zone.RowId.ToString());
            
            ImGui.TableNextColumn();
            ImGui.TextUnformatted(territoryName);

            ImGui.TableNextColumn();
            ImGui.TableNextColumn();
            if (GetDutyNameForTerritoryType(zoneId) is { } dutyName && !dutyName.IsNullOrEmpty())
            {
                ImGui.TextColored(KnownColor.Gray.Vector(), dutyName);
            }
            else
            {
                ImGui.Text(string.Empty);
            }
            
            ImGui.EndTable();
        }
    }
    
    private static List<SearchResult> Search(string searchTerms, int numResults) => LuminaCache<TerritoryType>.Instance
        .Where(territory => territory is { RowId: not 0, PlaceName.Value.RowId: not 0, QuestBattle.Row: 0, TerritoryIntendedUse: not 15 and not 29 })
        .Where(territory => territory.PlaceName.Value!.Name.ToDalamudString().TextValue.ToLowerInvariant().Contains(searchTerms.ToLowerInvariant()))
        .Select(territory => new SearchResult { TerritoryID = territory.RowId })
        .OrderBy(searchResult => searchResult.TerritoryName)
        .Take(numResults)
        .ToList();

    private static string? GetDutyNameForTerritoryType(uint territory)
    {
        if (LuminaCache<TerritoryType>.Instance.GetRow(territory) is not { ContentFinderCondition.Row: var cfcRow}) return null;
        if (LuminaCache<ContentFinderCondition>.Instance.GetRow(cfcRow) is not { Name: var dutyName }) return null;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dutyName.ToDalamudString().TextValue);
    }
}
