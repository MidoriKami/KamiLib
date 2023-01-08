using Dalamud.Game.ClientState.Conditions;
using FFXIVClientStructs.FFXIV.Client.Game;
using KamiLib.Caching;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Utilities;

public static class Condition
{
    public static bool IsBoundByDuty()
    {
        if(IsInIslandSanctuary()) return false;

        return Service.Condition[ConditionFlag.BoundByDuty] ||
               Service.Condition[ConditionFlag.BoundByDuty56] ||
               Service.Condition[ConditionFlag.BoundByDuty95];
    }

    public static bool IsInCombat() => Service.Condition[ConditionFlag.InCombat];
    public static bool IsInCutsceneOrQuestEvent() => IsInCutscene() || IsInQuestEvent();
    public static bool IsDutyRecorderPlayback() => Service.Condition[ConditionFlag.DutyRecorderPlayback];
    public static bool IsIslandDoingSomethingMode() => Service.GameGui.GetAddonByName("MJIPadGuide", 1) != nint.Zero;

    public static bool IsInCutscene()
    {
        return Service.Condition[ConditionFlag.OccupiedInCutSceneEvent] ||
               Service.Condition[ConditionFlag.WatchingCutscene] ||
               Service.Condition[ConditionFlag.WatchingCutscene78];
    }
    
    public static bool IsInQuestEvent()
    {
        if (IsInIslandSanctuary() && IsIslandDoingSomethingMode()) return false;

        return Service.Condition[ConditionFlag.OccupiedInQuestEvent];
    }

    public static bool IsBetweenAreas()
    {
        return Service.Condition[ConditionFlag.BetweenAreas] ||
               Service.Condition[ConditionFlag.BetweenAreas51];
    }

    public static bool IsInIslandSanctuary()
    {
        var territoryInfo = LuminaCache<TerritoryType>.Instance.GetRow(Service.ClientState.TerritoryType);
        if (territoryInfo is null) return false;
        
        // Island Sanctuary
        return territoryInfo.TerritoryIntendedUse == 49;
    }
    
    public static bool IsCrafting()
    {
        return Service.Condition[ConditionFlag.Crafting] ||
               Service.Condition[ConditionFlag.Crafting40];
    }

    public static bool IsCrossWorld()
    {
        return Service.Condition[ConditionFlag.ParticipatingInCrossWorldPartyOrAlliance];
    }

    public static bool IsInSanctuary()
    {
        return GameMain.IsInSanctuary();
    }

    public static bool CheckFlag(ConditionFlag flag)
    {
        return Service.Condition[flag];
    }
    
    public static bool IsGathering()
    {
        return Service.Condition[ConditionFlag.Gathering] ||
               Service.Condition[ConditionFlag.Gathering42];
    }
}