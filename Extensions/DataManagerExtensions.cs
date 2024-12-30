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
	NormalRaid,
	ChaoticAlliance,
}

public static class DataManagerExtensions {
	public static IEnumerable<ContentFinderCondition> GetSavageDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => GetDutyType(cfc) is DutyType.Savage);

	public static IEnumerable<ContentFinderCondition> GetUltimateDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => GetDutyType(cfc) is DutyType.Ultimate);

	public static IEnumerable<ContentFinderCondition> GetExtremeDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => GetDutyType(cfc) is DutyType.Extreme);

	public static IEnumerable<ContentFinderCondition> GetUnrealDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => GetDutyType(cfc) is DutyType.Unreal);

	public static IEnumerable<ContentFinderCondition> GetCriterionDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => GetDutyType(cfc) is DutyType.Criterion);

	public static IEnumerable<ContentFinderCondition> GetAllianceDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => GetDutyType(cfc) is DutyType.Alliance);

	// Warning, expensive operation, as this has to cross-reference multiple data sets.
	public static IEnumerable<ContentFinderCondition> GetLimitedAllianceRaidDuties(this IDataManager dataManager)
		=> dataManager.GetLimitedDuties()
			.Where(cfc => GetDutyType(cfc) is DutyType.Alliance);

	// Warning, expensive operation, as this has to cross-reference multiple data sets.
	public static IEnumerable<ContentFinderCondition> GetLimitedSavageRaidDuties(this IDataManager dataManager)
		=> dataManager.GetLimitedDuties()
			.Where(cfc => GetDutyType(cfc) is DutyType.Savage);
	
	// Warning, expensive operation, as this has to cross-reference multiple data sets.
	public static IEnumerable<ContentFinderCondition> GetLimitedNormalRaidDuties(this IDataManager dataManager)
		=> dataManager.GetLimitedDuties()
			.Where(cfc => GetDutyType(cfc) is DutyType.NormalRaid);
    
	private static IEnumerable<ContentFinderCondition> GetLimitedDuties(this IDataManager dataManager)
		=> dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English)
			.Where(cfc => dataManager.GetExcelSheet<InstanceContent>()
				.Where(instanceContent => instanceContent is { WeekRestriction: 1 })
				.Select(instanceContent => instanceContent.RowId)
				.Contains(cfc.Content.RowId));

	public static DutyType GetDutyType(this IDataManager dataManager, ContentFinderCondition cfc)
		=> GetDutyType(dataManager.GetExcelSheet<ContentFinderCondition>(ClientLanguage.English).GetRow(cfc.RowId));

	// Helper to check the duty type of entry that is already in english.
	private static DutyType GetDutyType(ContentFinderCondition cfc)
		=> cfc switch {
			{ ContentType.RowId: 5, ContentMemberType.RowId: 4 } => DutyType.Alliance,
			{ ContentType.RowId: 36 } => DutyType.ChaoticAlliance,
			{ ContentType.RowId: 5 } when cfc.Name.ExtractText().Contains("Savage") => DutyType.Savage,
			{ ContentType.RowId: 5 } when !cfc.Name.ExtractText().Contains("Savage") => DutyType.NormalRaid,
			{ ContentType.RowId: 28 } => DutyType.Ultimate,
			{ ContentType.RowId: 4 } when cfc.Name.ExtractText().Contains("Extreme") || cfc.Name.ExtractText().Contains("Minstrel") => DutyType.Extreme,
			{ ContentType.RowId: 4 } => DutyType.Unreal,
			{ ContentType.RowId: 30, AllowUndersized: false } => DutyType.Criterion,
			_ => DutyType.Unknown,
		};

	public static unsafe DutyType GetCurrentDutyType(this IDataManager dataManager) {
		var cfc = dataManager.GetExcelSheet<ContentFinderCondition>().GetRow(GameMain.Instance()->CurrentContentFinderConditionId);
		return dataManager.GetDutyType(cfc);
	}
}