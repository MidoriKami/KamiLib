using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Caching;

public class ActionCache : LuminaCache<Action>
{
    private static ActionCache? _instance;
    public static ActionCache Instance => _instance ??= new ActionCache();

    private ActionCache()
    {
        // Initialize Inherited Type
    }

}