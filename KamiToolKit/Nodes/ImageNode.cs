using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Nodes;

public unsafe class ImageNode : ResourceNode, IImageNode {
    public override AtkResNode* ResNode { get; protected set; }
    public override NodeType NodeType => NodeType.Image;

    private readonly AtkUnitBase* addon;
    private bool isDisposed;
    private readonly List<IAddonEventHandle?> clickHandles = new();

    public ImageNode(AtkUnitBase* addon) {
        this.addon = addon;

        AllocateImageNode();
        SetDefaults();
        
        Service.AddonLifecycle.RegisterListener(AddonEvent.PreFinalize, MemoryHelper.ReadStringNullTerminated((nint)addon->Name), AutoDispose);
    }
    
    private AtkImageNode* ContainedImageNode {
        get => (AtkImageNode*)ResNode; 
        set => ResNode = (AtkResNode*) value;
    }

    public byte WrapMode {
        get => ContainedImageNode->WrapMode;
        set => ContainedImageNode->WrapMode = value;
    }

    public ImageNodeFlags Flags {
        get => (ImageNodeFlags) ContainedImageNode->Flags;
        set => ContainedImageNode->Flags = (byte) value;
    }

    private Action? internalOnClick;
    public Action? OnClick {
        set {
            if (internalOnClick is null && value is not null) {
                clickHandles.AddRange(new List<IAddonEventHandle?>
                {
                    Service.EventManager.AddEvent((nint) addon, (nint) ResNode, AddonEventType.MouseOver, HandleOnClick),
                    Service.EventManager.AddEvent((nint) addon, (nint) ResNode, AddonEventType.MouseOut, HandleOnClick),
                    Service.EventManager.AddEvent((nint) addon, (nint) ResNode, AddonEventType.MouseClick, HandleOnClick),
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

    public void LoadTexture(string texturePath)
        => ContainedImageNode->LoadTexture(texturePath);

    public void LoadTexture(uint iconId)
        => ContainedImageNode->LoadIconTexture((int)iconId, 0);

    public void UnloadTexture()
        => ContainedImageNode->UnloadTexture();

    private void SetDefaults() {
        Color = Vector4.One;
        WrapMode = 1;
        Flags = ImageNodeFlags.AutoFit;
    }
    
    private void AllocateImageNode() {
        var newNode = IMemorySpace.GetUISpace()->Create<AtkImageNode>();

        var partsList = (AtkUldPartsList*) IMemorySpace.GetUISpace()->Malloc((ulong) sizeof(AtkUldPartsList), 8);
        partsList->Id = 1;
        partsList->Parts = null;
        partsList->PartCount = 1;

        var part = (AtkUldPart*) IMemorySpace.GetUISpace()->Malloc((ulong) sizeof(AtkUldPart), 8);
        part->U = 0;
        part->V = 0;
        part->Width = 0;
        part->Height = 0;

        var asset = (AtkUldAsset*) IMemorySpace.GetUISpace()->Malloc((ulong) sizeof(AtkUldAsset), 8);
        asset->Id = 1;
        asset->AtkTexture.Ctor();

        part->UldAsset = asset;
        partsList->Parts = part;
        newNode->PartsList = partsList;
        
        newNode->AtkResNode.NodeFlags = NodeFlags.Visible | NodeFlags.Enabled | NodeFlags.RespondToMouse | NodeFlags.HasCollision | NodeFlags.EmitsEvents;
        newNode->AtkResNode.Type = NodeType.Image;

        ContainedImageNode = newNode;
    }
    
    public override void Dispose() {
        if (!isDisposed) {
            Service.AddonLifecycle.UnregisterListener(AutoDispose);
            
            // These will trigger the events to be unregistered if they were in use.
            OnClick = null;
            
            IMemorySpace.Free(ContainedImageNode->PartsList->Parts->UldAsset, (ulong) sizeof(AtkUldAsset));
            IMemorySpace.Free(ContainedImageNode->PartsList->Parts, (ulong) sizeof(AtkUldPart));
            IMemorySpace.Free(ContainedImageNode->PartsList, (ulong) sizeof(AtkUldPartsList));

            ContainedImageNode->AtkResNode.Destroy(false);
            IMemorySpace.Free(ContainedImageNode, (ulong) sizeof(AtkImageNode));

            isDisposed = true;
        }
    }
    
    private void AutoDispose(AddonEvent type, AddonArgs args) 
        => Dispose();
    
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