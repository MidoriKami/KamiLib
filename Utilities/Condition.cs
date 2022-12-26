using Dalamud.Game.ClientState.Conditions;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Utilities;

public static class Condition
{
    private static readonly LuminaCache<TerritoryType> TerritoryTypeCache = new();

    public static bool IsBoundByDuty()
    {
        var baseBoundByDuty = Service.Condition[ConditionFlag.BoundByDuty];
        var boundBy56 = Service.Condition[ConditionFlag.BoundByDuty56];
        var boundBy95 = Service.Condition[ConditionFlag.BoundByDuty95];

        var territoryInfo = TerritoryTypeCache.GetRow(Service.ClientState.TerritoryType);

        // Island Sanctuary
        if (territoryInfo.TerritoryIntendedUse == 49)
            return false;

        return baseBoundByDuty || boundBy56 || boundBy95;
    }

    public static bool InCutsceneOrQuestEvent()
    {
        return Service.Condition[ConditionFlag.OccupiedInCutSceneEvent] ||
               Service.Condition[ConditionFlag.WatchingCutscene] ||
               Service.Condition[ConditionFlag.WatchingCutscene78] ||
               Service.Condition[ConditionFlag.OccupiedInQuestEvent];
    }

    public static bool BetweenAreas()
    {
        return Service.Condition[ConditionFlag.BetweenAreas] ||
               Service.Condition[ConditionFlag.BetweenAreas51];
    }
}