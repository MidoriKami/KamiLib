using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Nodes;

public unsafe class ImageNode : ResourceNode, IImageNode {
    public override AtkResNode* ResNode { get; protected set; }
    public override NodeType NodeType => NodeType.Image;

    public ImageNode() 
        => AllocateImageNode();

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

    public void LoadTexture(string texturePath)
        => ContainedImageNode->LoadTexture(texturePath);

    public void LoadTexture(uint iconId)
        => ContainedImageNode->LoadIconTexture((int)iconId, 0);

    public void UnloadTexture()
        => ContainedImageNode->UnloadTexture();

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
        
        Color = Vector4.One;
        WrapMode = 1;
        Flags = ImageNodeFlags.AutoFit;
    }
    
    public override void Dispose() {
        base.Dispose();

        IMemorySpace.Free(ContainedImageNode->PartsList->Parts->UldAsset, (ulong) sizeof(AtkUldAsset));
        IMemorySpace.Free(ContainedImageNode->PartsList->Parts, (ulong) sizeof(AtkUldPart));
        IMemorySpace.Free(ContainedImageNode->PartsList, (ulong) sizeof(AtkUldPartsList));

        ContainedImageNode->AtkResNode.Destroy(false);
        IMemorySpace.Free(ContainedImageNode, (ulong) sizeof(AtkImageNode));
    }
}