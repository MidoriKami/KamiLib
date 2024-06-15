using System.Collections.Generic;
using System.Linq;
using Dalamud;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets2;

namespace KamiLib.Extensions;

public enum DutyType {
    Unknown,
    Savage,
    Ultimate,
    Extreme,
    Unreal,
    Criterion,
    Alliance,
}

public static class DataManagerExtensions {
    public static IEnumerable<ContentFinderCondition> GetSavageDuties(this IDataManager dataManager)
        => dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)?
               .Where(cfc => cfc is { ContentType.Row: 5 })
               .Where(cfc => cfc.Name.RawString.Contains("Savage")) ?? [];

    public static IEnumerable<ContentFinderCondition> GetUltimateDuties(this IDataManager dataManager)
        => dataManager.GetExcelSheet<ContentFinderCondition>()?
               .Where(cfc => cfc is { ContentType.Row: 28 }) ?? [];

    public static IEnumerable<ContentFinderCondition> GetExtremeDuties(this IDataManager dataManager)
        => dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)?
               .Where(cfc => cfc is { ContentType.Row: 4, HighEndDuty: false }) ?? [];

    public static IEnumerable<ContentFinderCondition> GetUnrealDuties(this IDataManager dataManager)
        => dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)?
               .Where(cfc => cfc is { ContentType.Row: 4, HighEndDuty: true }) ?? [];

    public static IEnumerable<ContentFinderCondition> GetCriterionDuties(this IDataManager dataManager)
        => dataManager.GetExcelSheet<ContentFinderCondition>()?
               .Where(cfc => cfc is { ContentType.Row: 30, AllowUndersized: false }) ?? [];

    public static IEnumerable<ContentFinderCondition> GetAllianceDuties(this IDataManager dataManager)
        => dataManager.GetExcelSheet<ContentFinderCondition>()?
               .Where(cfc => cfc is { ContentType.Row: 5, ContentMemberType.Row: 4 }) ?? [];

    // Warning, expensive operation, as this has to cross-reference multiple data sets.
    public static IEnumerable<ContentFinderCondition> GetLimitedAllianceDuties(this IDataManager dataManager)
        => dataManager.GetLimitedDuties()
            .Where(cfc => cfc is { ContentType.Row: 5, ContentMemberType.Row: 4 });

    // Warning, expensive operation, as this has to cross-reference multiple data sets.
    public static IEnumerable<ContentFinderCondition> GetLimitedSavageDuties(this IDataManager dataManager)
        => dataManager.GetLimitedDuties()
            .Where(cfc => cfc is { ContentType.Row: 5 })
            .Where(cfc => cfc.Name.RawString.Contains("Savage"));
    
    private static IEnumerable<ContentFinderCondition> GetLimitedDuties(this IDataManager dataManager)
        => dataManager.GetExcelSheet<ContentFinderCondition>()?
            .Where(cfc => dataManager.GetExcelSheet<InstanceContent>()?
                .Where(instanceContent => instanceContent is { WeekRestriction: 1 })
                .Select(instanceContent => instanceContent.RowId)
                .Contains(cfc.Content.Row) ?? false) ?? [];

    public static DutyType GetDutyType(this IDataManager dataManager, ContentFinderCondition cfc) {
        var englishCfc = dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)!.GetRow(cfc.RowId);

        return englishCfc switch {
            { ContentType.Row: 5 } when englishCfc.Name.ToString().Contains("Savage") => DutyType.Savage,
            { ContentType.Row: 28 } => DutyType.Ultimate,
            { ContentType.Row: 4, HighEndDuty: false } => DutyType.Extreme,
            { ContentType.Row: 4, HighEndDuty: true } => DutyType.Unreal,
            { ContentType.Row: 30, AllowUndersized: false } => DutyType.Criterion,
            { ContentType.Row: 5, ContentMemberType.Row: 4 } => DutyType.Alliance,
            _ => DutyType.Unknown,
        };
    }

    public static unsafe DutyType GetCurrentDutyType(this IDataManager dataManager) {
        var cfc = dataManager.GetExcelSheet<ContentFinderCondition>()!.GetRow(GameMain.Instance()->CurrentContentFinderConditionId);
        if (cfc is null) return DutyType.Unknown;

        return dataManager.GetDutyType(cfc);
    }
}
