using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Caching;

public class PlaceNameCache : LuminaCache<PlaceName>
{
    private static PlaceNameCache? _instance;
    public static PlaceNameCache Instance => _instance ??= new PlaceNameCache();

    private PlaceNameCache()
    {
        // Initialize Inherited Type
    }
}