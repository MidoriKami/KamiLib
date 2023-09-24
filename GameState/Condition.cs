using Dalamud.Game.ClientState.Conditions;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.GameState;

public static class Condition
{
    public static bool IsBoundByDuty() => Service.Condition.Any(
        ConditionFlag.BoundByDuty,
        ConditionFlag.BoundByDuty56,
        ConditionFlag.BoundByDuty95);

    public static bool IsInCombat() 
        => Service.Condition[ConditionFlag.InCombat];
    
    public static bool IsInCutsceneOrQuestEvent() 
        => IsInCutscene() || IsInQuestEvent();
    
    public static bool IsDutyRecorderPlayback() 
        => Service.Condition[ConditionFlag.DutyRecorderPlayback];
    
    public static bool IsIslandDoingSomethingMode()
        => Service.GameGui.GetAddonByName("MJIPadGuide") != nint.Zero;

    public static bool IsInCutscene() => Service.Condition.Any(
        ConditionFlag.OccupiedInCutSceneEvent, 
        ConditionFlag.WatchingCutscene, 
        ConditionFlag.WatchingCutscene78);
    
    public static bool IsInQuestEvent() 
        => !IsIslandDoingSomethingMode() && Service.Condition[ConditionFlag.OccupiedInQuestEvent];

    public static bool IsBetweenAreas() => Service.Condition.Any(
        ConditionFlag.BetweenAreas,
        ConditionFlag.BetweenAreas51);

    public static bool IsInIslandSanctuary() 
        => LuminaCache<TerritoryType>.Instance.GetRow(Service.ClientState.TerritoryType)?.TerritoryIntendedUse is 49;

    public static bool IsCrafting() => Service.Condition.Any(
        ConditionFlag.Crafting, 
        ConditionFlag.Crafting40);
    
    public static bool IsCrossWorld() 
        => Service.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];
    
    public static bool IsGathering() => Service.Condition.Any(
        ConditionFlag.Gathering, 
        ConditionFlag.Gathering42);
    
    public static bool IsInBardPerformance()
        => Service.Condition[ConditionFlag.Performing];
}