using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.UserInterface.Native;

public static unsafe class Node
{
    public static T* GetNodeByID<T>(AtkUldManager uldManager, uint nodeId) where T : unmanaged 
    {
        foreach (var index in Enumerable.Range(0, uldManager.NodeListCount))
        {
            var currentNode = uldManager.NodeList[index];
            if (currentNode->NodeID != nodeId) continue;

            return (T*) currentNode;
        }
        
        return null;
    }
    
    public static void LinkNodeAtEnd(AtkResNode* resNode, AtkUnitBase* parent)
    {
        var node = parent->RootNode->ChildNode;
        while (node->PrevSiblingNode != null) node = node->PrevSiblingNode;

        node->PrevSiblingNode = resNode;
        resNode->NextSiblingNode = node;
        resNode->ParentNode = node->ParentNode;

        node->ChildCount++;
        
        parent->UldManager.UpdateDrawNodeList();
    }

    public static void UnlinkNodeAtEnd(AtkResNode* resNode, AtkUnitBase* parent)
    {
        if (resNode->PrevSiblingNode is not null)
        {
            resNode->PrevSiblingNode->NextSiblingNode = resNode->NextSiblingNode;
        }

        if (resNode->NextSiblingNode is not null)
        {
            resNode->NextSiblingNode->PrevSiblingNode = resNode->PrevSiblingNode;
        }
        
        parent->UldManager.UpdateDrawNodeList();
    }

    public static void LinkNodeAtStart(AtkResNode* resNode, AtkUnitBase* parent)
    {
        var rootNode = parent->RootNode;

        resNode->ParentNode = rootNode;
        resNode->PrevSiblingNode = rootNode->ChildNode;
        resNode->NextSiblingNode = null;

        if (rootNode->ChildNode->NextSiblingNode is not null)
        {
            rootNode->ChildNode->NextSiblingNode = resNode;
        }
        
        rootNode->ChildNode = resNode;

        parent->UldManager.UpdateDrawNodeList();
    }

    public static void UnlinkNodeAtStart(AtkResNode* resNode, AtkUnitBase* parent)
    {
        if (!IsAddonReady(parent)) return;
        if (parent->RootNode->ChildNode->NodeID != resNode->NodeID) return;
        
        var rootNode = parent->RootNode;
        
        if (resNode->PrevSiblingNode is not null)
        {
            resNode->PrevSiblingNode->NextSiblingNode = null;
        }
        
        rootNode->ChildNode = resNode->PrevSiblingNode;

        parent->UldManager.UpdateDrawNodeList();
    }

    public static bool IsAddonReady(AtkUnitBase* addon)
    {
        if (addon is null) return false;
        if (addon->RootNode is null) return false;
        if (addon->RootNode->ChildNode is null) return false;

        return true;
    }
}