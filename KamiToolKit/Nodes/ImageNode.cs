using System;
using System.Numerics;
using Dalamud.Game.Addon.Lifecycle;
using Dalamud.Game.Addon.Lifecycle.AddonArgTypes;
using Dalamud.Memory;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Controllers;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Nodes;

public unsafe class ImageNode : ResourceNode, IImageNode {
    public override AtkResNode* ResNode { get; protected set; }
    public override NodeType NodeType => NodeType.Image;

    private readonly ClickHandler clickHandler;
    private bool isDisposed;

    public ImageNode(AtkUnitBase* addon) {
        clickHandler = new ClickHandler(this, addon);

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

    public Action? OnClick {
        set => clickHandler.OnClick = value;
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
            
            clickHandler.Dispose();
            
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
}