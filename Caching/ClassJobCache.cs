using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Caching;

public class ClassJobCache : LuminaCache<ClassJob>
{
    private static ClassJobCache? _instance;
    public static ClassJobCache Instance => _instance ??= new ClassJobCache();

    private ClassJobCache()
    {
        // Initialize Inherited Type
    }
}