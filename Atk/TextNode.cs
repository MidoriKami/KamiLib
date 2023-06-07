using System;
using System.Numerics;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Interfaces;

namespace KamiLib.Atk;

public class TextNodeOptions
{
    public NodeType Type { get; set; } = NodeType.Text;
    public uint Id { get; set; }

    public Vector4 TextColor { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);
    public Vector4 EdgeColor { get; set; } = new(0.0f, 0.0f, 0.0f, 1.0f);
    public Vector4 BackgroundColor { get; set; } = new(1.0f, 1.0f, 1.0f, 1.0f);

    public AlignmentType Alignment { get; set; } = AlignmentType.Left;
    public byte FontSize { get; set; } = 12;
    public TextFlags Flags { get; set; } = TextFlags.AutoAdjustNodeSize | TextFlags.Bold | TextFlags.Edge;
}

public unsafe class TextNode : IDisposable, IAtkNode
{
    public AtkTextNode* Node { get; }
    
    public TextNode(TextNodeOptions options)
    {
        Node = IMemorySpace.GetUISpace()->Create<AtkTextNode>();
        
        Node->AtkResNode.Flags = (short) (NodeFlags.EmitsEvents | NodeFlags.Enabled | NodeFlags.AnchorLeft | NodeFlags.RespondToMouse | NodeFlags.HasCollision);
        UpdateOptions(options);
    }

    public void Dispose()
    {
        Node->AtkResNode.Destroy(false);
        IMemorySpace.Free(Node, (ulong)sizeof(AtkTextNode));
    }
    
    public void UpdateOptions(TextNodeOptions options)
    {
        Node->AtkResNode.Type = options.Type;
        Node->AtkResNode.NodeID = options.Id;
        Node->AtkResNode.SetHeight(options.FontSize);

        Node->TextColor = options.TextColor.ToByteColor();
        Node->EdgeColor = options.EdgeColor.ToByteColor();
        Node->BackgroundColor = options.BackgroundColor.ToByteColor();
        Node->AlignmentFontType = (byte) options.Alignment;
        Node->FontSize = options.FontSize;
        Node->TextFlags = (byte) options.Flags;
    }

    public AtkResNode* ResourceNode => (AtkResNode*) Node;
}