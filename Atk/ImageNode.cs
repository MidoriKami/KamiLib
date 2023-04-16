using System;
using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Interfaces;

namespace KamiLib.Atk;

public class ImageNodeOptions
{
    public NodeType Type { get; set; } = NodeType.Image;
    
    public uint Id { get; set; }
    
    public ImageNodeFlags Flags { get; set; } = ImageNodeFlags.AutoFit;
    public byte WrapMode { get; set; } = 1;
    
    public Vector4 Color { get; set; } = new(1.0f, 1.0f, 1.0f, 0.25f);
}

public unsafe class ImageNode : IDisposable, IAtkNode
{
    private readonly AtkImageNode* node;

    public ImageNode(ImageNodeOptions options)
    {
        node = IMemorySpace.GetUISpace()->Create<AtkImageNode>();
        
        var partsList = (AtkUldPartsList*) IMemorySpace.GetUISpace()->Malloc((ulong) sizeof(AtkUldPartsList), 8);
        partsList->Id = 1;
        partsList->Parts = null;
        partsList->PartCount = 1;
        
        var part = (AtkUldPart*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldPart), 8);
        part->U = 0;
        part->V = 0;
        part->Width = 0;
        part->Height = 0;
        
        var asset = (AtkUldAsset*)IMemorySpace.GetUISpace()->Malloc((ulong)sizeof(AtkUldAsset), 8);
        asset->Id = 1;
        asset->AtkTexture.Ctor();

        part->UldAsset = asset;
        partsList->Parts = part;
        node->PartsList = partsList;
        
        node->AtkResNode.Flags = (short) (NodeFlags.EmitsEvents | NodeFlags.Enabled | NodeFlags.AnchorLeft);
        node->AtkResNode.Color = options.Color.ToByteColor();
        UpdateOptions(options);
    }
    
    public void Dispose()
    {
        IMemorySpace.Free(node->PartsList->Parts->UldAsset, (ulong) sizeof(AtkUldAsset));
        IMemorySpace.Free(node->PartsList->Parts, (ulong)sizeof(AtkUldPart));
        IMemorySpace.Free(node->PartsList, (ulong)sizeof(AtkUldPartsList));
        
        node->AtkResNode.Destroy(false);
        IMemorySpace.Free(node, (ulong)sizeof(AtkImageNode));
    }

    public AtkResNode* GetResourceNode() => (AtkResNode*) node;

    public void LoadIcon(int iconId) => node->LoadIconTexture(iconId, 0);
    public void LoadTexture(string path) => node->LoadTexture(path);
    public void SetVisible(bool visible) => node->AtkResNode.ToggleVisibility(visible);

    public void UpdateOptions(ImageNodeOptions options)
    {
        node->AtkResNode.Type = options.Type;
        node->AtkResNode.NodeID = options.Id;
        node->WrapMode = options.WrapMode;
        node->Flags = (byte) options.Flags;

        node->AtkResNode.Color = options.Color.ToByteColor();
    }
}