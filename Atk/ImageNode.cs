using System.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Atk;

public static unsafe class ImageNode
{
    private static IMemorySpace* UISpace => IMemorySpace.GetUISpace();
    
    public static AtkImageNode* MakeNode(uint nodeId, Vector2 textureCoordinates, Vector2 textureSize)
    {
        var customNode = UISpace->Create<AtkImageNode>();
        customNode->AtkResNode.Type = NodeType.Image;
        customNode->AtkResNode.NodeID = nodeId;
        customNode->AtkResNode.Flags = 8243;
        customNode->AtkResNode.DrawFlags = 0;
        customNode->WrapMode = 1;
        customNode->Flags = 0;

        var partsList = MakePartsList(0, 1);
        if (partsList == null)
        {
            FreeImageNode(customNode);
            return null;
        }

        var part = MakePart(textureCoordinates, textureSize);
        if (part == null)
        {
            FreePartsList(partsList);
            FreeImageNode(customNode);
            return null;
        }
        
        partsList->Parts = part;

        var asset = MakeAsset(0);
        if (asset == null) 
        {
            FreePart(part);
            FreePartsList(partsList);
            FreeImageNode(customNode);
            return null;
        }
        
        part->UldAsset = asset;
        customNode->PartsList = partsList;
        
        return customNode;
    }

    private static AtkUldPartsList* MakePartsList(uint id, uint partCount)
    {
        var partsList = (AtkUldPartsList*) UISpace->Malloc((ulong) sizeof(AtkUldPartsList), 8);

        if (partsList is not null)
        {
            partsList->Id = id;
            partsList->PartCount = partCount;
            return partsList;
        }

        return null;
    }

    private static AtkUldPart* MakePart(Vector2 textureCoordinates, Vector2 size)
    {
        var part = (AtkUldPart*) UISpace->Malloc((ulong) sizeof(AtkUldPart), 8);

        if (part is not null)
        {
            part->U = (ushort) textureCoordinates.X;
            part->V = (ushort) textureCoordinates.Y;

            part->Width = (ushort) size.X;
            part->Height = (ushort) size.Y;
            return part;
        }

        return null;
    }

    private static AtkUldAsset* MakeAsset(uint id)
    {
        var asset = (AtkUldAsset*) UISpace->Malloc((ulong)sizeof(AtkUldAsset), 8);

        if (asset is not null)
        {
            asset->Id = id;
            asset->AtkTexture.Ctor();
            return asset;
        }

        return null;
    }
    
    private static void FreePartsList(AtkUldPartsList* partsList)
    {
        if (partsList is not null)
        {
            IMemorySpace.Free(partsList, (ulong) sizeof(AtkUldPartsList));
        }
    }

    private static void FreePart(AtkUldPart* part)
    {
        if (part is not null)
        {
            IMemorySpace.Free(part, (ulong) sizeof(AtkUldPart));
        }
    }

    private static void FreeAsset(AtkUldAsset* asset)
    {
        if (asset is not null)
        {
            asset->AtkTexture.Destroy(false);
            IMemorySpace.Free(asset, (ulong) sizeof(AtkUldAsset));
        }
    }
    
    public static void FreeImageNode(AtkImageNode* imageNode)
    {
        if (imageNode is not null)
        {
            var partsList = imageNode->PartsList;
            if (partsList is not null)
            {
                var part = imageNode->PartsList->Parts;
                if (part is not null)
                {
                    var asset = imageNode->PartsList->Parts->UldAsset;
                    if (asset is not null)
                    {
                        FreeAsset(asset);
                    }
                    
                    FreePart(part);
                }
                
                FreePartsList(partsList);
            }
            
            imageNode->AtkResNode.Destroy(false);
            IMemorySpace.Free(imageNode, (ulong) sizeof(AtkImageNode));
        }
    }

    public static void LinkNode(AtkComponentNode* rootNode, AtkResNode* beforeNode, AtkImageNode* newNode)
    {
        var prev = beforeNode->PrevSiblingNode;
        newNode->AtkResNode.ParentNode = beforeNode->ParentNode;

        beforeNode->PrevSiblingNode = (AtkResNode*) newNode;
        prev->NextSiblingNode = (AtkResNode*) newNode;

        newNode->AtkResNode.PrevSiblingNode = prev;
        newNode->AtkResNode.NextSiblingNode = beforeNode;

        rootNode->Component->UldManager.UpdateDrawNodeList();
    }
}