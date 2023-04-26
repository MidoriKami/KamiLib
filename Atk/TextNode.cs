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
    private readonly AtkTextNode* node;
    private readonly AtkTooltip tooltip = new();
    private bool tooltipEnabled;
    
    public TextNode(TextNodeOptions options)
    {
        node = IMemorySpace.GetUISpace()->Create<AtkTextNode>();
        
        node->AtkResNode.Flags = (short) (NodeFlags.EmitsEvents | NodeFlags.Enabled | NodeFlags.AnchorLeft | NodeFlags.RespondToMouse | NodeFlags.HasCollision);
        UpdateOptions(options);
    }

    public void Dispose()
    {
        if (tooltipEnabled)
        {
            tooltip.RemoveTooltip((AtkResNode*) node);
        }
        
        node->AtkResNode.Destroy(false);
        IMemorySpace.Free(node, (ulong)sizeof(AtkTextNode));
    }

    public void SetText(string text) => node->SetText(text);
    public void SetText(byte[] text) => node->SetText(text);
    public void SetVisible(bool visible) => node->AtkResNode.ToggleVisibility(visible);
    public void UpdateTooltip(string newTooltip) => tooltip.UpdateText(newTooltip);

    public void EnableTooltip(AtkUnitBase* parentAddon, string tooltipText)
    {
        if (tooltipEnabled) throw new Exception("Tooltip is already enabled");
        
        tooltip.AddTooltip(parentAddon, (AtkResNode*) node, tooltipText);
        tooltipEnabled = true;
    }
    
    public void UpdateOptions(TextNodeOptions options)
    {
        node->AtkResNode.Type = options.Type;
        node->AtkResNode.NodeID = options.Id;
        // node->AtkResNode.SetWidth((ushort) options.Size.X); // Width should be automatic
        node->AtkResNode.SetHeight(options.FontSize);
        // node->AtkResNode.SetPositionFloat(options.Position.X, options.Position.Y);

        node->TextColor = options.TextColor.ToByteColor();
        node->EdgeColor = options.EdgeColor.ToByteColor();
        node->BackgroundColor = options.BackgroundColor.ToByteColor();
        node->AlignmentFontType = (byte) options.Alignment;
        node->FontSize = options.FontSize;
        node->TextFlags = (byte) options.Flags;
    }
    
    public AtkResNode* ResourceNode => (AtkResNode*) node;
}