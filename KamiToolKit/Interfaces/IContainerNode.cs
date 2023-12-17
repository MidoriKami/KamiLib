using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Enums;

namespace KamiLib.KamiToolKit.Interfaces;

public unsafe interface IContainerNode {
    void AddNode(IResNode node, bool adjustPosition);
    void AddNode(IEnumerable<IResNode> nodes, bool adjustPosition);
    void RemoveNode(IResNode node);
    void RemoveNode(IEnumerable<IResNode> nodes);
    IResNode? GetNodeById(uint id);
    IResNode GetNodeByIndex(uint index);
    void LinkNode(AtkResNode* parentNode, NodePosition position);
}