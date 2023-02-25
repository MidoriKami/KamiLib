using Dalamud.Utility;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.ZoneFilterList;

public class SearchResult
{
    public uint TerritoryID { get; init; }
    private uint PlaceNameRow => LuminaCache<TerritoryType>.Instance.GetRow(TerritoryID)?.PlaceName.Row ?? 0;
    public string TerritoryName => LuminaCache<PlaceName>.Instance.GetRow(PlaceNameRow)?.Name.ToDalamudString().TextValue ?? "Unknown PlaceName Row";
}