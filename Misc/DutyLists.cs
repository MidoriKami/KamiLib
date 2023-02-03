using System.Collections.Generic;
using System.Linq;
using Dalamud;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Misc;

public enum DutyType
{
    Savage,
    Ultimate,
    ExtremeUnreal,
    Criterion,
    Alliance,
    None,
}

public class DutyLists
{
    public List<uint> Savage { get; }
    public List<uint> Ultimate { get; }
    public List<uint> ExtremeUnreal { get; }
    public List<uint> Criterion { get; }
    public List<uint> Alliance { get; }
    public List<uint> LimitedAlliance { get; }
    public List<uint> LimitedSavage { get; }
    
    private static DutyLists? _instance;
    public static DutyLists Instance => _instance ??= new DutyLists();

    private DutyLists()
    {
        // ContentType.Row 5 == Raids
        Savage = LuminaCache<ContentFinderCondition>.Instance.OfLanguage(ClientLanguage.English)
            .Where(t => t.ContentType.Row == 5)
            .Where(t => t.Name.RawString.Contains("Savage"))
            .Select(r => r.TerritoryType.Row)
            .ToList();
        
        // ContentType.Row 28 == Ultimate Raids
        Ultimate = LuminaCache<ContentFinderCondition>.Instance
            .Where(t => t.ContentType.Row == 28)
            .Select(t => t.TerritoryType.Row)
            .ToList();
        
        // ContentType.Row 4 == Trials
        ExtremeUnreal = LuminaCache<ContentFinderCondition>.Instance.OfLanguage(ClientLanguage.English)
            .Where(t => t.ContentType.Row == 4)
            .Where(t => t.Name.RawString.Contains("Extreme") || t.Name.RawString.Contains("Unreal") || t.Name.RawString.Contains("The Minstrel"))
            .Select(t => t.TerritoryType.Row)
            .ToList();

        Criterion = LuminaCache<ContentFinderCondition>.Instance
            .Where(row => row.ContentType.Row is 30)
            .Select(row => row.TerritoryType.Row)
            .ToList();
        
        Alliance = LuminaCache<TerritoryType>.Instance
            .Where(r => r.TerritoryIntendedUse is 8)
            .Select(r => r.RowId)
            .ToList();
        
        var instanceContents = LuminaCache<InstanceContent>.Instance
            .Where(instance => instance.WeekRestriction == 1)
            .Select(instance => instance.RowId);
        
        LimitedAlliance = LuminaCache<ContentFinderCondition>.Instance
            .Where(cfc => instanceContents.Contains(cfc.Content))
            .Where(cfc => cfc.TerritoryType.Value?.TerritoryIntendedUse is 8)
            .Select(cfc => cfc.TerritoryType.Row)
            .ToList();
        
        LimitedSavage = LuminaCache<ContentFinderCondition>.Instance.OfLanguage(ClientLanguage.English)
            .Where(cfc => instanceContents.Contains(cfc.Content))
            .Where(cfc => cfc.TerritoryType.Value?.TerritoryIntendedUse is 17)
            .Where(cfc => !cfc.Name.RawString.Contains("Ultimate"))
            .OrderByDescending(cfc => cfc.SortKey)
            .Select(cfc => cfc.TerritoryType.Row)
            .ToList();
    }

    private DutyType GetDutyType(uint dutyId)
    {
        if (Savage.Contains(dutyId)) return DutyType.Savage;
        if (Ultimate.Contains(dutyId)) return DutyType.Ultimate;
        if (ExtremeUnreal.Contains(dutyId)) return DutyType.ExtremeUnreal;
        if (Criterion.Contains(dutyId)) return DutyType.Criterion;
        if (Alliance.Contains(dutyId)) return DutyType.Alliance;

        return DutyType.None;
    }
    
    public bool IsType(uint dutyId, DutyType type) => GetDutyType(dutyId) == type;
    public bool IsType(uint dutyId, IEnumerable<DutyType> types) => types.Any(type => IsType(dutyId, type));
}