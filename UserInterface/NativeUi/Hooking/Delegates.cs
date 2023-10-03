using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.NativeUi;

public static unsafe class Delegates
{
    public delegate nint AgentReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* args, uint argCount, ulong sender);

    public delegate void AgentShow(AgentInterface* agent);
}