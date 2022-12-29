using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Caching;

public class OnlineStatusCache : LuminaCache<OnlineStatus>
{
    private static OnlineStatusCache? _instance;
    public static OnlineStatusCache Instance => _instance ??= new OnlineStatusCache();

    private OnlineStatusCache()
    {
        // Initialize Inherited Type
    }
}