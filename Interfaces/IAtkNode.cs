using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Interfaces;

public unsafe interface IAtkNode
{
    AtkResNode* ResourceNode { get; }
}