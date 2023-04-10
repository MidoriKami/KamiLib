using System;
using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Atk;

public class TextNodeOptions
{
    public NodeType Type { get; set; } = NodeType.Text;
    public required uint Id { get; set; }
    
    public required Vector4 TextColor { get; set; }
    public required Vector4 EdgeColor { get; set; }
    public required Vector4 BackgroundColor { get; set; }
    
    public required byte LineSpacing { get; set; }
    public required AlignmentType Alignment { get; set; }
    public required byte FontSize { get; set; }
    public byte TextFlags { get; set; } = 0x88;
    
    public required Vector2 Position { get; set; }
    public required Vector2 Size { get; set; }
}

public unsafe class TextNode : IDisposable
{
    private readonly AtkTextNode* node;
    private readonly AtkUnitBase* addonAtkUnitBase;
    
    public TextNode(TextNodeOptions options, AtkUnitBase* parentAddon)
    {
        addonAtkUnitBase = parentAddon;
        
        node = IMemorySpace.GetUISpace()->Create<AtkTextNode>();
        
        node->AtkResNode.Flags = (short) (NodeFlags.EmitsEvents | NodeFlags.Enabled | NodeFlags.AnchorLeft);
        UpdateOptions(options);

        Node.LinkNodeAtEnd((AtkResNode*) node, parentAddon);
    }

    public void Dispose()
    {
        if (node->AtkResNode.PrevSiblingNode is not null)
            node->AtkResNode.PrevSiblingNode->NextSiblingNode = node->AtkResNode.NextSiblingNode;
            
        if (node->AtkResNode.NextSiblingNode is not null)
            node->AtkResNode.NextSiblingNode->PrevSiblingNode = node->AtkResNode.PrevSiblingNode;
            
        addonAtkUnitBase->UldManager.UpdateDrawNodeList();
        
        node->AtkResNode.Destroy(false);
        IMemorySpace.Free(node, (ulong)sizeof(AtkTextNode));
    }

    public void SetText(byte[] text) => node->SetText(text);
    public void ToggleVisibility(bool visible) => node->AtkResNode.ToggleVisibility(visible);

    public void UpdateOptions(TextNodeOptions options)
    {
        node->AtkResNode.Type = options.Type;
        node->AtkResNode.NodeID = options.Id;
        node->AtkResNode.SetWidth((ushort) options.Size.X);
        node->AtkResNode.SetHeight((ushort) options.Size.Y);
        node->AtkResNode.SetPositionFloat(options.Position.X, options.Position.Y);

        node->TextColor = options.TextColor.ToByteColor();
        node->EdgeColor = options.EdgeColor.ToByteColor();
        node->BackgroundColor = options.BackgroundColor.ToByteColor();
        node->LineSpacing = options.LineSpacing;
        node->AlignmentFontType = (byte) options.Alignment;
        node->FontSize = options.FontSize;
        node->TextFlags = options.TextFlags;
    }
}