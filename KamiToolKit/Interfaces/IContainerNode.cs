using System;
using System.Collections.Generic;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Enums;

namespace KamiLib.KamiToolKit.Interfaces;

public unsafe interface IContainerNode : IDisposable, IEnumerable<IResNode> {
    void AddNode(IResNode node, AddNodeFlags flags);
    void RemoveNode(IResNode node);
    IResNode? GetNodeById(uint id);
    IResNode GetNodeByIndex(uint index);
    void LinkNode(AtkResNode* parentNode, NodePosition position);
    bool ShowBackground { get; set; }
    Vector2 ElementPadding { get; set; }
}