using System.Collections.Generic;
using System.Linq;
using Dalamud.Game;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.Sheets;

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
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => cfc is { ContentType.RowId: 5 })
			.Where(cfc => cfc.Name.ExtractText().Contains("Savage"));

	public static IEnumerable<ContentFinderCondition> GetUltimateDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>()
			.Where(cfc => cfc is { ContentType.RowId: 28 });

	public static IEnumerable<ContentFinderCondition> GetExtremeDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => cfc is { ContentType.RowId: 4 })
			.Where(cfc => cfc.Name.ExtractText().Contains("Extreme") || englishCfc.Name.ExtractText().Contains("Minstrel"));

	public static IEnumerable<ContentFinderCondition> GetUnrealDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => cfc is { ContentType.RowId: 4 });

	public static IEnumerable<ContentFinderCondition> GetCriterionDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>()
			.Where(cfc => cfc is { ContentType.RowId: 30, AllowUndersized: false });

	public static IEnumerable<ContentFinderCondition> GetAllianceDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>()
			.Where(cfc => cfc is { ContentType.RowId: 5, ContentMemberType.RowId: 4 });

	// Warning, expensive operation, as this has to cross-reference multiple data sets.
	public static IEnumerable<ContentFinderCondition> GetLimitedAllianceRaidDuties(this IDataManager dataManager)
		=> dataManager.GetLimitedDuties()
			.Where(cfc => cfc is { ContentType.RowId: 5, ContentMemberType.RowId: 4 });

	// Warning, expensive operation, as this has to cross-reference multiple data sets.
	public static IEnumerable<ContentFinderCondition> GetLimitedSavageRaidDuties(this IDataManager dataManager)
		=> dataManager.GetLimitedDuties()
			.Where(cfc => cfc is { ContentType.RowId: 5 })
			.Where(cfc => cfc.Name.ExtractText().Contains("Savage"));
	
	// Warning, expensive operation, as this has to cross-reference multiple data sets.
	public static IEnumerable<ContentFinderCondition> GetLimitedNormalRaidDuties(this IDataManager dataManager)
		=> dataManager.GetLimitedDuties()
			.Where(cfc => cfc is { ContentType.RowId: 5 })
			.Where(cfc => !cfc.Name.ExtractText().Contains("Savage"));
    
	private static IEnumerable<ContentFinderCondition> GetLimitedDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>()
			.Where(cfc => dataManager.GetExcelSheet<InstanceContent>()
				.Where(instanceContent => instanceContent is { WeekRestriction: 1 })
				.Select(instanceContent => instanceContent.RowId)
				.Contains(cfc.Content.RowId));

	public static DutyType GetDutyType(this IDataManager dataManager, ContentFinderCondition cfc) {
		var englishCfc = dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English).GetRow(cfc.RowId);

		return englishCfc switch {
			{ ContentType.RowId: 5 } when englishCfc.Name.ExtractText().Contains("Savage") => DutyType.Savage,
			{ ContentType.RowId: 28 } => DutyType.Ultimate,
			{ ContentType.RowId: 4 } when englishCfc.Name.ExtractText().Contains("Extreme") || englishCfc.Name.ExtractText().Contains("Minstrel") => DutyType.Extreme,
			{ ContentType.RowId: 4 } => DutyType.Unreal,
			{ ContentType.RowId: 30, AllowUndersized: false } => DutyType.Criterion,
			{ ContentType.RowId: 5, ContentMemberType.RowId: 4 } => DutyType.Alliance,
			_ => DutyType.Unknown,
		};
	}

	public static unsafe DutyType GetCurrentDutyType(this IDataManager dataManager) {
		var cfc = dataManager.GetExcelSheet<ContentFinderCondition>().GetRow(GameMain.Instance()->CurrentContentFinderConditionId);
		return dataManager.GetDutyType(cfc);
	}
}
