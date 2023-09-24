using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Hooking;

public static unsafe class Delegates
{
    public delegate void AgentShow(AgentInterface* agent);
    public delegate nint AgentReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* args, uint argCount, ulong sender);
}
