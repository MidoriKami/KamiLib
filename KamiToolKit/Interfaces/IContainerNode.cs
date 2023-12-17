using System;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Enums;

namespace KamiLib.KamiToolKit.Interfaces;

public unsafe interface IContainerNode : IDisposable, IEnumerable<IResNode> {
    void AddNode(IResNode node, bool offsetFromPreviousNode);
    void AddNode(IEnumerable<IResNode> nodes, bool offsetFromPreviousNode);
    void RemoveNode(IResNode node);
    void RemoveNode(IEnumerable<IResNode> nodes);
    IResNode? GetNodeById(uint id);
    IResNode GetNodeByIndex(uint index);
    void LinkNode(AtkResNode* parentNode, NodePosition position);
}