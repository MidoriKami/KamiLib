using System;
using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Interfaces;

namespace KamiLib.NativeUi;

public class ImageNodeOptions
{
    public NodeType Type { get; set; } = NodeType.Image;
    public uint Id { get; set; }
    public ImageNodeFlags Flags { get; set; } = ImageNodeFlags.AutoFit;
    public byte WrapMode { get; set; } = 1;
    public Vector4 Color { get; set; } = new(1.0f, 1.0f, 1.0f, 0.25f);
}

// todo: rebuild using DailyDuty
public unsafe class ImageNode : IDisposable, IAtkNode
{
    public AtkImageNode* Node { get; }
    public AtkResNode* ResourceNode => (AtkResNode*) Node;
    
    public ImageNode(ImageNodeOptions options)
    {
        Node = IMemorySpace.GetUISpace()->Create<AtkImageNode>();

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
        Node->PartsList = partsList;

        Node->AtkResNode.NodeFlags = NodeFlags.EmitsEvents | NodeFlags.Enabled | NodeFlags.AnchorLeft;
        Node->AtkResNode.Color = options.Color.ToByteColor();
        UpdateOptions(options);
    }

    public void Dispose()
    {
        IMemorySpace.Free(Node->PartsList->Parts->UldAsset, (ulong) sizeof(AtkUldAsset));
        IMemorySpace.Free(Node->PartsList->Parts, (ulong) sizeof(AtkUldPart));
        IMemorySpace.Free(Node->PartsList, (ulong) sizeof(AtkUldPartsList));

        Node->AtkResNode.Destroy(false);
        IMemorySpace.Free(Node, (ulong) sizeof(AtkImageNode));
    }

    public void UpdateOptions(ImageNodeOptions options)
    {
        Node->AtkResNode.Type = options.Type;
        Node->AtkResNode.NodeID = options.Id;
        Node->WrapMode = options.WrapMode;
        Node->Flags = (byte) options.Flags;

        Node->AtkResNode.Color = options.Color.ToByteColor();
    }
}