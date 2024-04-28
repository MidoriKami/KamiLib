using System.Collections.Generic;
using System.Linq;
using Dalamud;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets2;

namespace KamiLib.Classes;

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

    // Warning, expensive operation, as this has to cross reference multiple data sets.
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
};