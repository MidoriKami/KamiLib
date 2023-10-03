using System.Collections.Generic;
using System.Linq;
using Dalamud;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Game;

public enum DutyType
{
    Savage,
    Ultimate,
    ExtremeUnreal,
    Criterion,
    Alliance,
    None
}

public class DutyLists
{
    private static DutyLists? _instance;
    private List<uint>? alliance;
    private List<uint>? criterion;
    private List<uint>? extremeUnreal;
    private List<uint>? limitedAlliance;
    private List<uint>? limitedSavage;
    private List<uint>? savage;
    private List<uint>? ultimate;

    public List<uint> Savage => savage ??= LuminaCache<ContentFinderCondition>.Instance.OfLanguage(ClientLanguage.English)
        .Where(t => t.ContentType.Row is 5)
        .Where(t => t.Name.RawString.Contains("Savage"))
        .Select(r => r.TerritoryType.Row)
        .ToList();

    public List<uint> Ultimate => ultimate ??= LuminaCache<ContentFinderCondition>.Instance
        .Where(t => t.ContentType.Row is 28)
        .Select(t => t.TerritoryType.Row)
        .ToList();

    public List<uint> ExtremeUnreal => extremeUnreal ??= LuminaCache<ContentFinderCondition>.Instance.OfLanguage(ClientLanguage.English)
        .Where(t => t.ContentType.Row is 4)
        .Where(t => new[] { "Extreme", "Unreal", "The Minstrel" }.Any(s => t.Name.RawString.Contains(s)))
        .Select(t => t.TerritoryType.Row)
        .ToList();

    public List<uint> Criterion => criterion ??= LuminaCache<ContentFinderCondition>.Instance
        .Where(row => row.ContentType.Row is 30)
        .Select(row => row.TerritoryType.Row)
        .ToList();

    public List<uint> Alliance => alliance ??= LuminaCache<TerritoryType>.Instance
        .Where(r => r.TerritoryIntendedUse is 8)
        .Select(r => r.RowId)
        .ToList();

    public List<uint> LimitedAlliance => limitedAlliance ??= LuminaCache<ContentFinderCondition>.Instance
        .Where(cfc => LuminaCache<InstanceContent>.Instance
            .Where(instance => instance.WeekRestriction == 1)
            .Select(instance => instance.RowId).Contains(cfc.Content))
        .Where(cfc => cfc.TerritoryType.Value?.TerritoryIntendedUse is 8)
        .Select(cfc => cfc.TerritoryType.Row)
        .ToList();

    public List<uint> LimitedSavage => limitedSavage ??= LuminaCache<ContentFinderCondition>.Instance.OfLanguage(ClientLanguage.English)
        .Where(cfc => LuminaCache<InstanceContent>.Instance
            .Where(instance => instance.WeekRestriction == 1)
            .Select(instance => instance.RowId).Contains(cfc.Content))
        .Where(cfc => cfc.TerritoryType.Value?.TerritoryIntendedUse is 17)
        .Where(cfc => !cfc.Name.RawString.Contains("Ultimate"))
        .OrderByDescending(cfc => cfc.SortKey)
        .Select(cfc => cfc.TerritoryType.Row)
        .ToList();

    public static DutyLists Instance => _instance ??= new DutyLists();

    private DutyType GetDutyType(uint dutyId)
    {
        if (Savage.Contains(dutyId)) return DutyType.Savage;
        if (Ultimate.Contains(dutyId)) return DutyType.Ultimate;
        if (ExtremeUnreal.Contains(dutyId)) return DutyType.ExtremeUnreal;
        if (Criterion.Contains(dutyId)) return DutyType.Criterion;
        if (Alliance.Contains(dutyId)) return DutyType.Alliance;

        return DutyType.None;
    }

    public bool IsType(uint dutyId, DutyType type)
    {
        return GetDutyType(dutyId) == type;
    }
    
    public bool IsType(uint dutyId, params DutyType[] types)
    {
        return types.Any(type => IsType(dutyId, type));
    }
}