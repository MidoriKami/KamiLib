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
    public AtkResNode* Node { get; }
    public AtkResNode* ResourceNode => Node;

    public ResNode(ResNodeOptions options)
    {
        Node = IMemorySpace.GetUISpace()->Create<AtkResNode>();

        Node->Type = NodeType.Res;
        Node->NodeID = options.Id;
        Node->NodeFlags = NodeFlags.Enabled | NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.IsTopNode;
        Node->SetWidth((ushort) options.Size.X);
        Node->SetHeight((ushort) options.Size.Y);
        Node->SetPositionFloat(options.Position.X, options.Position.Y);
    }

    public void Dispose()
    {
        Node->Destroy(false);
        IMemorySpace.Free(Node, (ulong)sizeof(AtkResNode));
    }

    public void AddResourceNode(IAtkNode newNode, AtkUnitBase* container)
    {
        var newResNode = newNode.ResourceNode;

        // If the child list is empty
        if (Node->ChildNode is null && Node->ChildCount is 0)
        {
            Node->ChildNode = newResNode;
            newResNode->ParentNode = Node;
            newResNode->PrevSiblingNode = null;
            newResNode->NextSiblingNode = null;
            newResNode->ChildNode = null;
            Node->ChildCount++;
        }
        // Else Add to the List
        else
        {
            var currentNode = Node->ChildNode;
            while (currentNode is not null && currentNode->PrevSiblingNode != null)
            {
                currentNode = currentNode->PrevSiblingNode;
            }
            
            newResNode->ParentNode = Node;
            newResNode->NextSiblingNode = currentNode;
            currentNode->PrevSiblingNode = newResNode;
            newResNode->NextSiblingNode = null;
            newResNode->ChildNode = null;
            Node->ChildCount++;
        }
        
        container->UldManager.UpdateDrawNodeList();
    }
}