using System.Linq;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.ChatCommands;

namespace KamiLib.Atk;

public unsafe class BaseNode
{
    private readonly AtkUnitBase* node;

    public bool NodeValid => node != null;
    
    public BaseNode(string addon)
    {
        node = (AtkUnitBase*) Service.GameGui.GetAddonByName(addon, 1);
    }

    public BaseNode Print()
    {
        Chat.Print("AtkUnitBase", $"{new nint(node):X8}");
        return this;
    }

    public AtkResNode* GetRootNode() => node == null ? null : node->RootNode;

    public T* GetNode<T>(uint id) where T : unmanaged
    {
        if (node == null) return null;

        var targetNode = (T*) node->GetNodeById(id);

        return targetNode;
    }

    public ComponentNode GetComponentNode(uint id)
    {
        if (node == null) return new ComponentNode(null);

        var targetNode = (AtkComponentNode*) node->GetNodeById(id);

        return new ComponentNode(targetNode);
    }

    public ComponentNode GetNestedNode(params uint[] idList)
    {
        uint index = 0;

        ComponentNode startingNode;

        do
        {
            startingNode = GetComponentNode(idList[index]);

        } while (index++ < idList.Length);
        
        return startingNode;
    }
}

public unsafe class ComponentNode
{
    private readonly AtkComponentNode* node;
    private readonly AtkComponentBase* componentBase;

    public ComponentNode(AtkComponentNode* node)
    {
        this.node = node;

        componentBase = node == null ? null : node->Component;
    }

    public ComponentNode Print()
    {
        Chat.Print("AtkComponentNode", $"{new nint(node):X8}");
        return this;
    }

    public ComponentNode GetComponentNode(uint id)
    {
        if (componentBase == null) return new ComponentNode(null);

        var targetNode = Node.GetNodeByID<AtkComponentNode>(componentBase->UldManager, id);

        return new ComponentNode(targetNode);
    }

    public T* GetNode<T>(uint id) where T : unmanaged => componentBase == null ? null : Node.GetNodeByID<T>(componentBase->UldManager, id);

    public AtkComponentNode* GetPointer() => node;
}

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