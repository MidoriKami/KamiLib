using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Caching;

public class TerritoryTypeCache : LuminaCache<TerritoryType>
{
    private static TerritoryTypeCache? _instance;
    public static TerritoryTypeCache Instance => _instance ??= new TerritoryTypeCache();

    private TerritoryTypeCache()
    {
        // Initialize Inherited Type
    }
}