using System;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Enums;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.NativeUi;

public static unsafe class NodeHelper {
    public static void InsertNode(IResNode node, AtkResNode* targetNode, NodePosition position)
        => InsertNode(node.ResNode, targetNode, position);
    
    private static void InsertNode(AtkResNode* newNode, AtkResNode* targetNode, NodePosition position) {
        switch (position) {
            case NodePosition.BeforeTarget:
                EmplaceBefore(newNode, targetNode);
                break;

            case NodePosition.AfterTarget:
                EmplaceAfter(newNode, targetNode);
                break;

            case NodePosition.BeforeAllSiblings:
                EmplaceBeforeSiblings(newNode, targetNode);
                break;

            case NodePosition.AfterAllSiblings:
                EmplaceAfterSiblings(newNode, targetNode);
                break;
            
            case NodePosition.AsLastChild:
                EmplaceAsLastChild(newNode, targetNode);
                break;
            
            case NodePosition.AsFirstChild:
                EmplaceAsFirstChild(newNode, targetNode);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }
    }

    private static void EmplaceBefore(AtkResNode* newNode, AtkResNode* targetNode) {
        newNode->ParentNode = targetNode->ParentNode;

        // Target node is the head of the nodelist, we will be the new head.
        if (targetNode->NextSiblingNode is null) {
            targetNode->ParentNode->ChildNode = newNode;
        }

        // We have a node that will be before us
        if (targetNode->NextSiblingNode is not null) {
            targetNode->NextSiblingNode->PrevSiblingNode = newNode;
            newNode->NextSiblingNode = targetNode->NextSiblingNode;
        }

        targetNode->NextSiblingNode = newNode;
        newNode->PrevSiblingNode = targetNode;
        
        targetNode->ParentNode->ChildCount++;
    }

    private static void EmplaceAfter(AtkResNode* newNode, AtkResNode* targetNode) {
        newNode->ParentNode = targetNode->ParentNode;

        // We have a node that will be after us
        if (targetNode->PrevSiblingNode is not null) {
            targetNode->PrevSiblingNode->NextSiblingNode = newNode;
            newNode->PrevSiblingNode = targetNode->PrevSiblingNode;
        }

        targetNode->PrevSiblingNode = newNode;
        newNode->NextSiblingNode = targetNode;
        
        targetNode->ParentNode->ChildCount++;
    }

    private static void EmplaceBeforeSiblings(AtkResNode* newNode, AtkResNode* targetNode) {
        var current = targetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->NextSiblingNode;
        }

        if (previous is not null) {
            EmplaceBefore(newNode, previous);
        }
        
        targetNode->ParentNode->ChildCount++;
    }

    private static void EmplaceAfterSiblings(AtkResNode* newNode, AtkResNode* targetNode) {
        var current = targetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->PrevSiblingNode;
        }

        if (previous is not null) {
            EmplaceAfter(newNode, previous);
        }

        targetNode->ParentNode->ChildCount++;
    }

    private static void EmplaceAsLastChild(AtkResNode* newNode, AtkResNode* targetNode) {
        // If the child list is empty
        if (targetNode->ChildNode is null)
        {
            targetNode->ChildNode = newNode;
            newNode->ParentNode = targetNode;
            targetNode->ChildCount++;
        }
        // Else Add to the List
        else
        {
            var currentNode = targetNode->ChildNode;
            while (currentNode is not null && currentNode->PrevSiblingNode != null)
            {
                currentNode = currentNode->PrevSiblingNode;
            }
        
            newNode->ParentNode = targetNode;
            newNode->NextSiblingNode = currentNode;
            currentNode->PrevSiblingNode = newNode;
            targetNode->ChildCount++;
        }
    }
    
    private static void EmplaceAsFirstChild(AtkResNode* newNode, AtkResNode* targetNode) {
        // If the child list is empty
        if (targetNode->ChildNode is null && targetNode->ChildCount is 0)
        {
            targetNode->ChildNode = newNode;
            newNode->ParentNode = targetNode;
            targetNode->ChildCount++;
        }
        // Else Add to the List as the First Child
        else {
            targetNode->ChildNode->NextSiblingNode = newNode;
            newNode->PrevSiblingNode = targetNode->ChildNode;
            targetNode->ChildNode = newNode;
            newNode->ParentNode = targetNode;
            targetNode->ChildCount++;
        }
    }

    public static void UnlinkNode(IResNode node)
        => UnlinkNode(node.ResNode);
    
    private static void UnlinkNode(AtkResNode* node) {
        if (node is null) return;
        if (node->ParentNode is null) return;
        
        // If we were the main child of the containing node, assign it to the next element in line.
        if (node->ParentNode->ChildNode == node) {
            // And we have a node after us, our parents child should be the next node in line.
            if (node->PrevSiblingNode != null) {
                node->ParentNode->ChildNode = node->PrevSiblingNode;
            }
            // else our parent is no longer pointing to any children.
            else {
                node->ParentNode->ChildNode = null;
            }
        }
        
        // If we have a node before us
        if (node->NextSiblingNode != null) {
            // and a node after us, link the one before to the one after
            if (node->PrevSiblingNode != null) {
                node->NextSiblingNode->PrevSiblingNode = node->PrevSiblingNode;
            }
            // else unlink it from us
            else {
                node->NextSiblingNode->PrevSiblingNode = null;
            }
        }
        
        // If we have a node after us
        if (node->PrevSiblingNode != null) {
            // and a node before us, link the one after to the one before
            if (node->NextSiblingNode != null) {
                node->PrevSiblingNode->NextSiblingNode = node->NextSiblingNode;
            }
            // else unlink it from us
            else {
                node->PrevSiblingNode->NextSiblingNode = null;
            }
        }
    }

    public static bool IsAddonReady(AtkUnitBase* addon) {
        if (addon is null) return false;
        if (addon->RootNode is null) return false;
        if (addon->RootNode->ChildNode is null) return false;

        return true;
    }
}