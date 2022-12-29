using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Caching;

public class ItemCache : LuminaCache<Item>
{
    private static ItemCache? _instance;
    public static ItemCache Instance => _instance ??= new ItemCache();

    private ItemCache()
    {
        // Initialize Inherited Type
    }
}