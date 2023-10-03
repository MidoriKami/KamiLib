using Dalamud.Game.ClientState.Conditions;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Game;

public static class Condition
{
    public static bool IsBoundByDuty()
    {
        return Service.Condition.Any(
            ConditionFlag.BoundByDuty,
            ConditionFlag.BoundByDuty56,
            ConditionFlag.BoundByDuty95);
    }

    public static bool IsInCombat()
    {
        return Service.Condition[ConditionFlag.InCombat];
    }

    public static bool IsInCutsceneOrQuestEvent()
    {
        return IsInCutscene() || IsInQuestEvent();
    }

    public static bool IsDutyRecorderPlayback()
    {
        return Service.Condition[ConditionFlag.DutyRecorderPlayback];
    }

    public static bool IsIslandDoingSomethingMode()
    {
        return Service.GameGui.GetAddonByName("MJIPadGuide") != nint.Zero;
    }

    public static bool IsInCutscene()
    {
        return Service.Condition.Any(
            ConditionFlag.OccupiedInCutSceneEvent,
            ConditionFlag.WatchingCutscene,
            ConditionFlag.WatchingCutscene78);
    }

    public static bool IsInQuestEvent()
    {
        return !IsIslandDoingSomethingMode() && Service.Condition[ConditionFlag.OccupiedInQuestEvent];
    }

    public static bool IsBetweenAreas()
    {
        return Service.Condition.Any(
            ConditionFlag.BetweenAreas,
            ConditionFlag.BetweenAreas51);
    }

    public static bool IsInIslandSanctuary()
    {
        return LuminaCache<TerritoryType>.Instance.GetRow(Service.ClientState.TerritoryType)?.TerritoryIntendedUse is 49;
    }

    public static bool IsCrafting()
    {
        return Service.Condition.Any(
            ConditionFlag.Crafting,
            ConditionFlag.Crafting40);
    }

    public static bool IsCrossWorld()
    {
        return Service.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];
    }

    public static bool IsGathering()
    {
        return Service.Condition.Any(
            ConditionFlag.Gathering,
            ConditionFlag.Gathering42);
    }

    public static bool IsInBardPerformance()
    {
        return Service.Condition[ConditionFlag.Performing];
    }
}