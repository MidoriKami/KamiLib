using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Game.Addon.Events;
using Dalamud.Utility.Numerics;
using FFXIVClientStructs.FFXIV.Client.System.Memory;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.Interfaces;

namespace KamiLib.Atk;

// todo: rebuild using DailyDuty
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

    private Action? onClick;
    private Func<string>? getTooltip;

    private bool onClickEnabled;
    private bool tooltipEnabled;

    private readonly List<IAddonEventHandle?> tooltipHandles = new();
    private readonly List<IAddonEventHandle?> clickHandles = new();
    
    public TextNode(TextNodeOptions options)
    {
        Node = IMemorySpace.GetUISpace()->Create<AtkTextNode>();
        
        Node->AtkResNode.NodeFlags = NodeFlags.Enabled | NodeFlags.RespondToMouse | NodeFlags.HasCollision | NodeFlags.EmitsEvents;
        UpdateOptions(options);
    }

    public void Dispose()
    {
        Node->AtkResNode.Destroy(false);
        IMemorySpace.Free(Node, (ulong)sizeof(AtkTextNode));
    }

    public void AddTooltip(AtkUnitBase* parentAddon)
    {
        tooltipHandles.AddRange(new List<IAddonEventHandle?>
        {
            Service.EventManager.AddEvent((nint) parentAddon, (nint) Node, AddonEventType.MouseOver, HandleTooltip),
            Service.EventManager.AddEvent((nint) parentAddon, (nint) Node, AddonEventType.MouseOut, HandleTooltip),
        });

        tooltipEnabled = true;
    }
    
    public void AddClickEvent(AtkUnitBase* parentAddon, Action onClickAction)
    {
        clickHandles.AddRange(new List<IAddonEventHandle?>
        {
            Service.EventManager.AddEvent((nint) parentAddon, (nint) Node, AddonEventType.MouseOver, HandleOnClick),
            Service.EventManager.AddEvent((nint) parentAddon, (nint) Node, AddonEventType.MouseOut, HandleOnClick),
            Service.EventManager.AddEvent((nint) parentAddon, (nint) Node, AddonEventType.MouseClick, HandleOnClick),
        });

        onClick = onClickAction;
        onClickEnabled = true;
    }
    
    public void RemoveTooltip()
    {
        foreach (var tooltipHandle in tooltipHandles.OfType<IAddonEventHandle>())
        {
            Service.EventManager.RemoveEvent(tooltipHandle);
        }
        tooltipEnabled = false;
    }
    
    public void RemoveClickEvent()
    {
        foreach (var tooltipHandle in clickHandles.OfType<IAddonEventHandle>())
        {
            Service.EventManager.RemoveEvent(tooltipHandle);
        }
        onClickEnabled = false;
    }
    
    public void SetTooltipStringFunction(Func<string> getTooltipFunc) => getTooltip = getTooltipFunc;

    public void ToggleTooltip(bool enabled) => tooltipEnabled = enabled;
    
    public void ToggleClickEvent(bool enabled) => onClickEnabled = enabled;
    
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
    
    private void HandleTooltip(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode)
    {
        var addon = (AtkUnitBase*) atkUnitBase;
        var node = (AtkResNode*) atkResNode;

        if (getTooltip is not null && tooltipEnabled)
        {
            switch (atkEventType)
            {
                case AddonEventType.MouseOver:
                    AtkStage.GetSingleton()->TooltipManager.ShowTooltip(addon->ID, node, getTooltip.Invoke());
                    break;
            
                case AddonEventType.MouseOut:
                    AtkStage.GetSingleton()->TooltipManager.HideTooltip(addon->ID);
                    break;
            }
        }
    }

    private void HandleOnClick(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode)
    {
        if (onClick is not null && onClickEnabled)
        {
            switch (atkEventType)
            {
                case AddonEventType.MouseOver:
                    Service.EventManager.SetCursor(AddonCursorType.Clickable);
                    break;
            
                case AddonEventType.MouseOut:
                    Service.EventManager.ResetCursor();
                    break;
            
                case AddonEventType.MouseClick:
                    onClick.Invoke();
                    break;
            }
        }
    }

    public AtkResNode* ResourceNode => (AtkResNode*) Node;
}