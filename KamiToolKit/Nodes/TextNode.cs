using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Interface;
using Dalamud.Memory;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Nodes;

public unsafe class TextNode : ResourceNode, ITextNode {
    public override AtkResNode* ResNode { get; protected set; }
    public override NodeType NodeType => NodeType.Text;

    private readonly AtkUnitBase* addon;
    private bool isDisposed;
    private readonly List<IAddonEventHandle?> clickHandles = new();
    private readonly List<IAddonEventHandle?> tooltipHandles = new();

    private AtkTextNode* ContainedTextNode {
        get => (AtkTextNode*)ResNode; 
        set => ResNode = (AtkResNode*) value;
    }
    
    public TextNode(AtkUnitBase* addon) {
        this.addon = addon;

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

    private string? internalTooltip;
    public string? Tooltip {
        set {
            if (internalTooltip is null && value is not null) {
                tooltipHandles.AddRange(new List<IAddonEventHandle?>
                {
                    Service.EventManager.AddEvent((nint) addon, (nint) ResNode, AddonEventType.MouseOver, HandleTooltip),
                    Service.EventManager.AddEvent((nint) addon, (nint) ResNode, AddonEventType.MouseOut, HandleTooltip)
                });

                internalTooltip = value;
            }
            else if (internalTooltip is not null && value is null) {
                foreach (var tooltipHandle in tooltipHandles.OfType<IAddonEventHandle>()) {
                    Service.EventManager.RemoveEvent(tooltipHandle);
                }
                tooltipHandles.Clear();

                internalTooltip = null;
            }
        }
    }

    private Action? internalOnClick;
    public Action? OnClick {
        set {
            if (internalOnClick is null && value is not null) {
                clickHandles.AddRange(new List<IAddonEventHandle?>
                {
                    Service.EventManager.AddEvent((nint) addon, (nint) ResNode, AddonEventType.MouseOver, HandleOnClick),
                    Service.EventManager.AddEvent((nint) addon, (nint) ResNode, AddonEventType.MouseOut, HandleOnClick),
                    Service.EventManager.AddEvent((nint) addon, (nint) ResNode, AddonEventType.MouseClick, HandleOnClick)
                });

                internalOnClick = value;
            }
            else if (internalOnClick is not null && value is null) {
                foreach (var clickHandle in clickHandles.OfType<IAddonEventHandle>()) {
                    Service.EventManager.RemoveEvent(clickHandle);
                }
                clickHandles.Clear();

                internalOnClick = null;
            }
        }
    }
    
    public override void Dispose() {
        if (!isDisposed) {
            Service.AddonLifecycle.UnregisterListener(AutoDispose);

            // These will trigger the events to be unregistered if they were in use.
            Tooltip = null;
            OnClick = null;
            
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
    
    private void HandleTooltip(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
        var node = (AtkResNode*) atkResNode;

        if (internalTooltip is not null) {
            switch (atkEventType) {
                case AddonEventType.MouseOver:
                    AtkStage.GetSingleton()->TooltipManager.ShowTooltip(addon->ID, node, internalTooltip);
                    break;

                case AddonEventType.MouseOut:
                    AtkStage.GetSingleton()->TooltipManager.HideTooltip(addon->ID);
                    break;
            }
        }
    }
    
    private void HandleOnClick(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
        if (internalOnClick is not null) {
            switch (atkEventType) {
                case AddonEventType.MouseOver:
                    Service.EventManager.SetCursor(AddonCursorType.Clickable);
                    break;

                case AddonEventType.MouseOut:
                    Service.EventManager.ResetCursor();
                    break;

                case AddonEventType.MouseClick:
                    internalOnClick.Invoke();
                    break;
            }
        }
    }
}