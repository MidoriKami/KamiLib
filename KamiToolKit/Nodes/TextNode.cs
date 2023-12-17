using System;
using System.Drawing;
using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Memory;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Controllers;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Nodes;

public unsafe class TextNode : ResourceNode, ITextNode {
    public override AtkResNode* ResNode { get; protected set; }
    public override NodeType NodeType => NodeType.Text;

    private readonly TooltipHandler tooltipHandler;
    private readonly ClickHandler clickHandler;
    private bool isDisposed;

    private AtkTextNode* ContainedTextNode {
        get => (AtkTextNode*)ResNode; 
        set => ResNode = (AtkResNode*) value;
    }
    
    public TextNode(AtkUnitBase* addon) {
        tooltipHandler = new TooltipHandler(this, addon);
        clickHandler = new ClickHandler(this, addon);

        AllocateTextNode();
        SetDefaults();

        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, MemoryHelper.ReadStringNullTerminated((nint)addon->Name), AutoDispose);
    }

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

    public SeString? Tooltip {
        set => tooltipHandler.Text = value;
    }

    public Action? OnClick {
        set => clickHandler.OnClick = value;
    }
    
    public override void Dispose() {
        if (!isDisposed) {
            Service.AddonLifecycle.UnregisterListener(AutoDispose);

            tooltipHandler.Dispose();
            clickHandler.Dispose();
            
            ResNode->Destroy(false);
            IMemorySpace.Free(ResNode, (ulong) sizeof(AtkTextNode));
            isDisposed = true;
        }
    }
    
    private void SetDefaults() {
        Text = "Node Text Not Set";
        FontSize = 12;
        TextColor = KnownColor.White.Vector();
        OutlineColor = Vector4.Zero;
        BackgroundColor = KnownColor.White.Vector();
        TextAlignment = AlignmentType.Left;
        StyleFlags = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge;
    }
    
    private void AllocateTextNode() {
        var newNode = IMemorySpace.GetUISpace()->Create<AtkTextNode>();

        newNode->AtkResNode.NodeFlags = NodeFlags.Enabled | NodeFlags.RespondToMouse | NodeFlags.HasCollision | NodeFlags.EmitsEvents;
        newNode->AtkResNode.Type = NodeType.Text;

        ContainedTextNode = newNode;
    }
    
    private void AutoDispose(AddonEvent type, AddonArgs args) 
        => Dispose();
}