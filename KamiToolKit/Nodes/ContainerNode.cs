using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Enums;
using KamiLib.KamiToolKit.Interfaces;
using KamiLib.NativeUi;

namespace KamiLib.KamiToolKit.Nodes;

public unsafe class ContainerNode : ResourceNode, IContainerNode {
    public override AtkResNode* ResNode { get; protected set; }
    public override NodeType NodeType => NodeType.Res;
    private ImageNode? Background { get; set; }

    private readonly List<IResNode> containedNodes = new();
    public required uint NodeIdBase { get; init; }
    private uint currentNodeCount;

    /// <remarks>This node takes ownership of contained nodes, and when this node is disposed, it will dispose all contained nodes.</remarks>
    public ContainerNode() {
        AllocateResNode();
    }

    public void AddNode(IResNode node, AddNodeFlags flags = AddNodeFlags.None) {
        NodeHelper.InsertNode(node, ResNode, NodePosition.AsLastChild);
        UpdateDrawLists();

        Height = Math.Max(Height, node.Height);
        Width = Math.Max(Width, node.Width);
        
        if (flags.HasFlag(AddNodeFlags.IncrementNodeId)) {
            node.ResNode->NodeID = NodeIdBase + ++currentNodeCount;
        }

        // Skip the Background Node, we don't want to offset relative to the background.
        if (containedNodes.Skip(1).Any() && flags.HasFlag(AddNodeFlags.OffsetPosition)) {
            var previousNode = containedNodes.Last();
            node.Y = previousNode.Y + previousNode.Height + ElementPadding.Y;
        }

        if (flags.HasFlag(AddNodeFlags.FillParent)) {
            // node.Position = Vector2.Zero;
            node.Size = Size;
        }
        else {
            if (flags.HasFlag(AddNodeFlags.AdjustSize)) {
                node.Size = Size;
            }
            else {
                if (flags.HasFlag(AddNodeFlags.AdjustHeight)) {
                    node.Height = Height;
                } else if (flags.HasFlag(AddNodeFlags.AdjustWidth)) {
                    node.Width = Width;
                }
            }
        }
        
        containedNodes.Add(node);
    }

    public void RemoveNode(IResNode resNode) {
        UpdateDrawLists();
    }

    public IResNode? GetNodeById(uint id) 
        => containedNodes.FirstOrDefault(node => node.ResNode->NodeID == id);

    public IResNode GetNodeByIndex(uint index)
        => containedNodes[(int)index];
    
    public void LinkNode(AtkResNode* parentNode, NodePosition position) {
        NodeHelper.InsertNode(this, parentNode, position);
        UpdateDrawLists();
    }

    public new float Width {
        get => ResNode->Width;
        set {
            ResNode->Width = (ushort) value;
            
            if (Background is not null) {
                Background.Width = value + 10.0f;
            }
        }
    }
    
    public new float Height {
        get => ResNode->Height;
        set {
            ResNode->Height = (ushort) value;

            if (Background is not null) {
                Background.Height = value + 10.0f;
            }
        }
    }
    
    public bool ShowBackground {
        get => Background?.Visible ?? false;
        set {
            if (Background is null) {
                Background = new ImageNode {
                    ParentAddon = ParentAddon,
                    Position = new Vector2(-5.0f, -5.0f),
                    Size = Size + new Vector2(10.0f),
                    Color = new Vector4(0.3f, 0.3f, 0.3f, 0.3f),
                };
        
                AddNode(Background, AddNodeFlags.IncrementNodeId);
            }
            
            Background.Visible = value;
        }
    }

    public Vector2 ElementPadding { get; set; }

    public override void Dispose() {
        base.Dispose();
        
        foreach (var node in containedNodes) {
            RemoveNode(node);
            node.Dispose();
        }

        ResNode->Destroy(false);
        IMemorySpace.Free(ResNode, (ulong) sizeof(AtkResNode));
    }
    
    private void UpdateDrawLists() {
        ParentAddon->UldManager.UpdateDrawNodeList();
        ParentAddon->UpdateCollisionNodeList(false);
    }
    
    private void AllocateResNode() {
        ResNode = IMemorySpace.GetUISpace()->Create<AtkResNode>();
        
        ResNode->NodeFlags = NodeFlags.Enabled | NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.IsTopNode;
        ResNode->Type = NodeType.Res;
        ResNode->NodeID = NodeIdBase;
    }
    
    public IEnumerator<IResNode> GetEnumerator() 
        => containedNodes.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();
}