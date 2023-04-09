using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Hooking;

public static unsafe class Delegates
{
    public delegate nint AddonOnSetup(AtkUnitBase* addon, int valueCount, AtkValue* values);
    public delegate void AddonDraw(AtkUnitBase* addon);
    public delegate byte AddonOnRefresh(AtkUnitBase* addon, int valueCount, AtkValue* values);
    public delegate void AddonFinalize(AtkUnitBase* addon);
    public delegate byte AddonUpdate(AtkUnitBase* addon);

    public delegate void AgentShow(AgentInterface* agent);
    public delegate nint AgentReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* args, uint argCount, ulong sender);
}
