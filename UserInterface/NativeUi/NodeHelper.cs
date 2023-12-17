using System;
using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Enums;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.NativeUi;

public static unsafe class Node {
    public static T* GetNodeByID<T>(AtkUldManager uldManager, uint nodeId) where T : unmanaged {
        foreach (var index in Enumerable.Range(0, uldManager.NodeListCount)) {
            var currentNode = uldManager.NodeList[index];
            if (currentNode->NodeID != nodeId) continue;

            return (T*) currentNode;
        }

        return null;
    }

    public static void InsertNode(IResNode newNode, AtkResNode* targetNode, NodePosition position) {
        switch (position) {
            case NodePosition.BeforeTarget:
                EmplaceBefore(newNode, targetNode);
                break;

            case NodePosition.AfterTarget:
                EmplaceAfter(newNode, targetNode);
                break;

            case NodePosition.BeforeEverything:
                EmplaceBeforeAll(newNode, targetNode);
                break;

            case NodePosition.AfterEverything:
                EmplaceAfterAll(newNode, targetNode);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(position), position, null);
        }
    }

    private static void EmplaceBefore(IResNode newNode, AtkResNode* targetNode) {
        newNode.ResNode->ParentNode = targetNode->ParentNode;

        // Target node is the head of the nodelist, we will be the new head.
        if (targetNode->NextSiblingNode is null) {
            targetNode->ParentNode->ChildNode = newNode.ResNode;
        }

        // We have a node that will be before us
        if (targetNode->NextSiblingNode is not null) {
            targetNode->NextSiblingNode->PrevSiblingNode = newNode.ResNode;
            newNode.ResNode->NextSiblingNode = targetNode->NextSiblingNode;
        }

        targetNode->NextSiblingNode = newNode.ResNode;
        newNode.ResNode->PrevSiblingNode = targetNode;
    }

    private static void EmplaceAfter(IResNode newNode, AtkResNode* targetNode) {
        newNode.ResNode->ParentNode = targetNode->ParentNode;

        // We have a node that will be after us
        if (targetNode->PrevSiblingNode is not null) {
            targetNode->PrevSiblingNode->NextSiblingNode = newNode.ResNode;
            newNode.ResNode->PrevSiblingNode = targetNode->PrevSiblingNode;
        }

        targetNode->PrevSiblingNode = newNode.ResNode;
        newNode.ResNode->NextSiblingNode = targetNode;
    }

    private static void EmplaceBeforeAll(IResNode newNode, AtkResNode* targetNode) {
        var current = targetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->NextSiblingNode;
        }

        if (previous is not null) {
            EmplaceBefore(newNode, previous);
        }
    }

    private static void EmplaceAfterAll(IResNode newNode, AtkResNode* targetNode) {
        var current = targetNode;
        var previous = current;

        while (current is not null) {
            previous = current;
            current = current->PrevSiblingNode;
        }

        if (previous is not null) {
            EmplaceAfter(newNode, previous);
        }
    }
    
    public static void UnlinkNode(IResNode resNode) {
        if (resNode.ResNode is null) return;
        var node = resNode.ResNode;

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