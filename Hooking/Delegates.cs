using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Hooking;

public static class Delegates
{
    public static unsafe class Addon
    {
        public delegate nint OnSetup(AtkUnitBase* addon, int valueCount, AtkValue* values);
        public delegate void Draw(AtkUnitBase* addon);
        public delegate byte OnRefresh(AtkUnitBase* addon, int valueCount, AtkValue* values);
        public delegate void Finalize(AtkUnitBase* addon);
        public delegate byte Update(AtkUnitBase* addon);
    }

    public static unsafe class Agent
    {
        public delegate void Show(AgentInterface* agent);
        public delegate nint ReceiveEvent(AgentInterface* agent, nint rawData, AtkValue* args, uint argCount, ulong sender);
    }

    public static unsafe class Other
    {
        public delegate void* GoldSaucerUpdate(void* a1, byte* a2, uint a3, ushort a4, void* a5, int* data, byte eventID);
    }
}
