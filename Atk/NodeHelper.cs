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
}