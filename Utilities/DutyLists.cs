using System.Collections.Generic;
using System.Linq;
using Dalamud;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Utilities;

public enum DutyType
{
    Savage,
    Ultimate,
    ExtremeUnreal,
    Criterion,
    None,
}

public class DutyLists
{
    private List<uint> Savage { get; init; }
    private List<uint> Ultimate { get; init; }
    private List<uint> ExtremeUnreal { get; init; }
    private List<uint> Criterion { get; init; }

    private static DutyLists? _instance;
    public static DutyLists Instance => _instance ??= new DutyLists();
    
    public DutyLists()
    {
        // ContentType.Row 5 == Raids
        Savage = Service.DataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)!
            .Where(t => t.ContentType.Row == 5)
            .Where(t => t.Name.RawString.Contains("Savage"))
            .Select(r => r.TerritoryType.Row)
            .ToList();
        
        // ContentType.Row 28 == Ultimate Raids
        Ultimate = Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
            .Where(t => t.ContentType.Row == 28)
            .Select(t => t.TerritoryType.Row)
            .ToList();
        
        // ContentType.Row 4 == Trials
        ExtremeUnreal = Service.DataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)!
            .Where(t => t.ContentType.Row == 4)
            .Where(t => t.Name.RawString.Contains("Extreme") || t.Name.RawString.Contains("Unreal") || t.Name.RawString.Contains("The Minstrel"))
            .Select(t => t.TerritoryType.Row)
            .ToList();

        Criterion = Service.DataManager.GetExcelSheet<ContentFinderCondition>()!
            .Where(row => row.ContentType.Row is 30)
            .Select(row => row.RowId)
            .ToList();
    }

    public DutyType GetDutyType(uint dutyId)
    {
        if (Savage.Contains(dutyId)) return DutyType.Savage;
        if (Ultimate.Contains(dutyId)) return DutyType.Ultimate;
        if (ExtremeUnreal.Contains(dutyId)) return DutyType.ExtremeUnreal;
        if (Criterion.Contains(dutyId)) return DutyType.Criterion;

        return DutyType.None;
    }
}