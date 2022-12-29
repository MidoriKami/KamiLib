using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Caching;

public class CompanyActionCache : LuminaCache<CompanyAction>
{
    private static CompanyActionCache? _instance;
    public static CompanyActionCache Instance => _instance ??= new CompanyActionCache();

    private CompanyActionCache()
    {
        // Initialize Inherited Type
    }
}