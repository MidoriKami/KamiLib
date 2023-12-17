using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Enums;
using KamiLib.KamiToolKit.Interfaces;
using KamiLib.NativeUi;

namespace KamiLib.KamiToolKit.Nodes;

public unsafe class ContainerNode : ResourceNode, IContainerNode {
    public override AtkResNode* ResNode { get; protected set; }
    public override NodeType NodeType => NodeType.Res;

    private readonly uint nodeId;
    private readonly List<IResNode> containedNodes = new();
    private readonly AtkUnitBase* addon;
    private readonly bool disposeChildren;
    private bool isDisposed;

    public ContainerNode(uint id, AtkUnitBase* parentAddon, bool disposeContainedNodes = true) {
        nodeId = id;
        addon = parentAddon;
        disposeChildren = disposeContainedNodes;
        
        AllocateResNode(nodeId);
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, MemoryHelper.ReadStringNullTerminated((nint)addon->Name), AutoDispose);
    }

    public void AddNode(IResNode node, bool offsetFromPreviousNode = true) {
        NodeHelper.InsertNode(node, ResNode, NodePosition.AsLastChild);
        UpdateDrawLists();

        node.ResNode->NodeID = nodeId + (uint)containedNodes.Count + 1u;

        if (containedNodes.Any() && offsetFromPreviousNode) {
            var previousNode = containedNodes.Last();
            node.Y = previousNode.Y + previousNode.Height;
        }
        
        containedNodes.Add(node);
    }

    public void AddNode(IEnumerable<IResNode> nodes, bool offsetFromPreviousNode = true) {
        foreach (var node in nodes){
            AddNode(node, offsetFromPreviousNode);
        }
    }

    public void RemoveNode(IResNode resNode) {
        NodeHelper.UnlinkNode(resNode);
        UpdateDrawLists();
    }
    
    public void RemoveNode(IEnumerable<IResNode> nodes) {
        foreach (var node in nodes) {
            RemoveNode(node);
        }
    }

    public IResNode? GetNodeById(uint id) 
        => containedNodes.FirstOrDefault(node => node.ResNode->NodeID == id);

    public IResNode GetNodeByIndex(uint index)
        => containedNodes[(int)index];
    
    public void LinkNode(AtkResNode* parentNode, NodePosition position) {
        NodeHelper.InsertNode(this, parentNode, position);
        UpdateDrawLists();
    }
    
    public override void Dispose() {
        if (!isDisposed) {
            Service.AddonLifecycle.UnregisterListener(AutoDispose);
        
            RemoveNode(containedNodes);

            if (disposeChildren) {
                foreach (var node in containedNodes) {
                    node.Dispose();
                }
            }
        
            NodeHelper.UnlinkNode(this);
            ResNode->Destroy(false);
            IMemorySpace.Free(ResNode, (ulong) sizeof(AtkResNode));
            
            isDisposed = true;
        }
    }
    
    private void UpdateDrawLists() {
        addon->UldManager.UpdateDrawNodeList();
        addon->UpdateCollisionNodeList(false);
    }
    
    private void AllocateResNode(uint baseNodeId) {
        ResNode = IMemorySpace.GetUISpace()->Create<AtkResNode>();
        
        ResNode->NodeFlags = NodeFlags.Enabled | NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.IsTopNode;
        ResNode->Type = NodeType.Res;
        ResNode->NodeID = baseNodeId;
    }
    
    private void AutoDispose(AddonEvent type, AddonArgs args) 
        => Dispose();

    public IEnumerator<IResNode> GetEnumerator() 
        => containedNodes.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}