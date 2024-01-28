using System.Drawing;
using System.Numerics;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Nodes;

public unsafe class TextNode : ResourceNode, ITextNode {
    public override AtkResNode* ResNode { get; protected set; }
    public override NodeType NodeType => NodeType.Text;

    private AtkTextNode* ContainedTextNode {
        get => (AtkTextNode*)ResNode; 
        set => ResNode = (AtkResNode*) value;
    }
    
    public TextNode() 
        => AllocateTextNode();

    public SeString Text {
        get => SeString.Parse(ContainedTextNode->NodeText);
        set => ContainedTextNode->SetText(value.Encode());
    }

    public byte FontSize {
        get => ContainedTextNode->FontSize;
        set => ContainedTextNode->FontSize = value;
    }

    public Vector4 TextColor {
        get => new(ContainedTextNode->TextColor.R, ContainedTextNode->TextColor.G, ContainedTextNode->TextColor.B, ContainedTextNode->TextColor.A);
        set => ContainedTextNode->TextColor = value.ToByteColor();
    }
    
    public Vector4 OutlineColor {
        get => new(ContainedTextNode->EdgeColor.R, ContainedTextNode->EdgeColor.G, ContainedTextNode->EdgeColor.B, ContainedTextNode->EdgeColor.A);
        set => ContainedTextNode->EdgeColor = value.ToByteColor();
    }
    
    public Vector4 BackgroundColor {
        get => new(ContainedTextNode->BackgroundColor.R, ContainedTextNode->BackgroundColor.G, ContainedTextNode->BackgroundColor.B, ContainedTextNode->BackgroundColor.A);
        set => ContainedTextNode->BackgroundColor = value.ToByteColor();
    }

    public AlignmentType TextAlignment {
        get => ContainedTextNode->AlignmentType;
        set => ContainedTextNode->AlignmentType = value;
    }

    public TextFlags StyleFlags {
        get => (TextFlags) ContainedTextNode->TextFlags;
        set => ContainedTextNode->TextFlags = (byte) value;
    }

    public void AddTextFlags(TextFlags flags) 
        => StyleFlags |= flags;

    public void RemoveTextFlags(TextFlags flags) 
        => StyleFlags &= ~flags;
    
    public override void Dispose() {
        base.Dispose();
        
        if (ResNode is not null) {
            ResNode->Destroy(false);
            IMemorySpace.Free(ResNode, (ulong) sizeof(AtkTextNode));

            ResNode = null;
        }
    }
    
    private void AllocateTextNode() {
        var newNode = IMemorySpace.GetUISpace()->Create<AtkTextNode>();

        newNode->AtkResNode.NodeFlags = NodeFlags.Enabled | NodeFlags.RespondToMouse | NodeFlags.HasCollision | NodeFlags.EmitsEvents;
        newNode->AtkResNode.Type = NodeType.Text;

        ContainedTextNode = newNode;
        
        StyleFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge;
        Text = "Node Text Not Set";
        FontSize = 12;
        Height = FontSize + 2.0f;
        TextColor = KnownColor.White.Vector();
        OutlineColor = Vector4.Zero;
        BackgroundColor = KnownColor.White.Vector();
        TextAlignment = AlignmentType.Left;
    }
}