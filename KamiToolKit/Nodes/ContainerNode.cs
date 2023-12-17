using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using FFXIVClientStructs.Interop;
using KamiLib.KamiToolKit.Enums;
using KamiLib.KamiToolKit.Interfaces;
using KamiLib.NativeUi;

namespace KamiLib.KamiToolKit.Nodes;

public unsafe class ContainerNode : ResourceNode, IContainerNode, IEnumerable<Pointer<AtkResNode>> {
    public override AtkResNode* ResNode { get; protected set; }
    public override NodeType NodeType => NodeType.Res;

    private readonly uint nodeId;
    private readonly List<IResNode> containedNodes = new();
    private readonly AtkUnitBase* addon;
    private bool isDisposed;

    public ContainerNode(uint id, AtkUnitBase* parentAddon) {
        nodeId = id;
        addon = parentAddon;
        
        AllocateResNode(nodeId);
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, MemoryHelper.ReadStringNullTerminated((nint)addon->Name), AutoDispose);
    }

    public void AddNode(IResNode node, bool adjustNodePosition = true) {
        InsertNodeIntoManagedList(node);

        var nodeIndex = (uint)containedNodes.Count + 1u;
        node.ResNode->NodeID = nodeId + nodeIndex;

        if (containedNodes.Any() && adjustNodePosition) {
            var previousNode = containedNodes.Last();
            node.Y = containedNodes.Count + 1 * previousNode.Height;
        }
        
        containedNodes.Add(node);
    }

    public void AddNode(IEnumerable<IResNode> nodes, bool adjustNodePosition = true) {
        foreach (var node in nodes){
            AddNode(node, adjustNodePosition);
        }
    }

    public void RemoveNode(IResNode resNode) {
        Node.UnlinkNode(resNode);
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
        Node.InsertNode(this, parentNode, position);
        UpdateDrawLists();
    }
    
    public void Dispose() {
        if (!isDisposed) {
            Service.AddonLifecycle.UnregisterListener(AutoDispose);
        
            RemoveNode(containedNodes);
        
            Node.UnlinkNode(this);
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

    public IEnumerator<Pointer<AtkResNode>> GetEnumerator() 
        => containedNodes.Select<IResNode, Pointer<AtkResNode>>(node => node.ResNode).GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
    
    private void InsertNodeIntoManagedList(IResNode node) {

        var newResNode = node.ResNode;

        // If the child list is empty
        if (ResNode->ChildNode is null && ResNode->ChildCount is 0)
        {
            ResNode->ChildNode = newResNode;
            newResNode->ParentNode = ResNode;
            newResNode->PrevSiblingNode = null;
            newResNode->NextSiblingNode = null;
            newResNode->ChildNode = null;
        }
        // Else Add to the List
        else
        {
            var currentNode = ResNode->ChildNode;
            while (currentNode is not null && currentNode->PrevSiblingNode != null)
            {
                currentNode = currentNode->PrevSiblingNode;
            }
        
            newResNode->ParentNode = ResNode;
            newResNode->NextSiblingNode = currentNode;
            currentNode->PrevSiblingNode = newResNode;
            newResNode->NextSiblingNode = null;
            newResNode->ChildNode = null;
        }
        
        UpdateDrawLists();
    }
}