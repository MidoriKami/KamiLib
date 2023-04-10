using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Atk;

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
        
        parent->UldManager.UpdateDrawNodeList();
    }
}