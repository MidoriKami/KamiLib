using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Caching;

public class StatusCache : LuminaCache<Status>
{
    private static StatusCache? _instance;
    public static StatusCache Instance => _instance ??= new StatusCache();

    private StatusCache()
    {
        // Initialize Inherited Type
    }
}