using System;
using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Interfaces;

namespace KamiLib.Atk;

public class ResNodeOptions
{
    public required uint Id { get; set; }
    public required Vector2 Size { get; set; }
    public required Vector2 Position { get; set; }
}

public unsafe class ResNode : IDisposable, IAtkNode
{
    private readonly AtkResNode* node;

    public ResNode(ResNodeOptions options)
    {
        node = IMemorySpace.GetUISpace()->Create<AtkResNode>();

        node->Type = NodeType.Res;
        node->NodeID = options.Id;
        node->Flags = (short) (NodeFlags.Enabled | NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.IsTopNode);
        node->SetWidth((ushort) options.Size.X);
        node->SetHeight((ushort) options.Size.Y);
        node->SetPositionFloat(options.Position.X, options.Position.Y);
    }

    public void Dispose()
    {
        node->Destroy(false);
        IMemorySpace.Free(node, (ulong)sizeof(AtkResNode));
    }

    public void SetVisibility(bool visible) => node->ToggleVisibility(visible);

    public void AddResourceNode(IAtkNode newNode, AtkUnitBase* container)
    {
        var newResNode = newNode.GetResourceNode();

        // If the child list is empty
        if (node->ChildNode is null && node->ChildCount is 0)
        {
            node->ChildNode = newResNode;
            newResNode->ParentNode = node;
            newResNode->PrevSiblingNode = null;
            newResNode->NextSiblingNode = null;
            newResNode->ChildNode = null;
            node->ChildCount++;
        }
        // Else Add to the List
        else
        {
            var currentNode = node->ChildNode;
            while (currentNode is not null && currentNode->PrevSiblingNode != null)
            {
                currentNode = currentNode->PrevSiblingNode;
            }
            
            newResNode->ParentNode = node;
            newResNode->NextSiblingNode = currentNode;
            currentNode->PrevSiblingNode = newResNode;
            newResNode->NextSiblingNode = null;
            newResNode->ChildNode = null;
            node->ChildCount++;
        }
        
        container->UldManager.UpdateDrawNodeList();
    }
    public AtkResNode* GetResourceNode() => node;
}