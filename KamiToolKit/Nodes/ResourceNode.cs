using System.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Nodes;

public abstract unsafe class ResourceNode : IResNode {
    public abstract AtkResNode* ResNode { get; protected set; }

    public virtual NodeType NodeType => NodeType.Res;

    public uint NodeId {
        get => ResNode->NodeID;
        set => ResNode->NodeID = value;
    }
    
    public void Enable() 
        => ResNode->NodeFlags = NodeFlags.Enabled | NodeFlags.Visible | NodeFlags.AnchorLeft | NodeFlags.IsTopNode;
    
    public void Disable()
        => ResNode->NodeFlags = NodeFlags.AnchorLeft | NodeFlags.IsTopNode;
    
    public void Toggle(bool enabled) {
        if (enabled) Enable();
        else Disable();
    }

    public Vector2 Size {
        get => new(Width, Height);
        set {
            Width = value.X;
            Height = value.Y;
        }
    }

    public float Width {
        get => ResNode->GetWidth();
        set => ResNode->SetWidth((ushort) value);
    }

    public float Height {
        get => ResNode->GetHeight();
        set => ResNode->SetHeight((ushort) value);
    }

    public Vector2 Position {
        get => new(X, Y);
        set => ResNode->SetPositionFloat(value.X, value.Y);
    }

    public float X {
        get => ResNode->X;
        set => ResNode->SetPositionFloat(value, ResNode->Y);
    }

    public float Y {
        get => ResNode->Y;
        set => ResNode->SetPositionFloat(ResNode->X, value);
    }
}