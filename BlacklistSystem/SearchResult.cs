using Dalamud.Utility;
using KamiLib.Caching;

namespace KamiLib.BlacklistSystem;

public class SearchResult
{
    public uint TerritoryID { get; init; }
    private uint PlaceNameRow => TerritoryTypeCache.Instance.GetRow(TerritoryID)?.PlaceName.Row ?? 0;
    public string TerritoryName => PlaceNameCache.Instance.GetRow(PlaceNameRow)?.Name.ToDalamudString().TextValue ?? "Unknown PlaceName Row";
}