using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Nodes;

public abstract unsafe class ResourceNode : IResNode {
    public abstract AtkResNode* ResNode { get; protected set; }

    public virtual NodeType NodeType => NodeType.Res;

    public abstract void Dispose();
    
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

    public bool Visible {
        get => ResNode->IsVisible;
        set => ResNode->ToggleVisibility(value);
    }

    public Vector4 Color {
        get => new(ResNode->Color.R, ResNode->Color.G, ResNode->Color.B, ResNode->Color.A);
        set => ResNode->Color = value.ToByteColor();
    }

    public Vector3 AddColor {
        get => new(ResNode->AddRed, ResNode->AddGreen, ResNode->AddBlue);
        set {
            ResNode->AddRed = (short) value.X;
            ResNode->AddGreen = (short) value.Y;
            ResNode->AddBlue = (short) value.Z;
        }
    }
    
    public Vector3 MultiplyColor {
        get => new(ResNode->MultiplyRed, ResNode->MultiplyGreen, ResNode->MultiplyBlue);
        set {
            ResNode->MultiplyRed = (byte) value.X;
            ResNode->MultiplyGreen = (byte) value.Y;
            ResNode->MultiplyBlue = (byte) value.Z;
        }
    }

    public float ScaleX {
        get => ResNode->ScaleX;
        set => ResNode->ScaleX = value;
    }

    public float ScaleY {
        get => ResNode->ScaleY;
        set => ResNode->ScaleY = value;
    }
    
    public Vector2 Scale {
        get => new(ScaleX, ScaleY);
        set {
            ScaleX = value.X;
            ScaleY = value.Y;
        }
    }

    public float Rotation {
        get => ResNode->Rotation;
        set => ResNode->Rotation = value;
    }
}