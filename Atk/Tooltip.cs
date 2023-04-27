using System;
using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Atk;

public unsafe class Tooltip : IDisposable
{
    private readonly nint tooltipMemory;
    private AtkResNode* attachedNode;
    
    public Tooltip()
    {
        tooltipMemory = Marshal.AllocHGlobal(4096);
    }

    public void AddTooltip(AtkUnitBase* parentAddon, AtkResNode* tooltipOwner, string text)
    {
        if (attachedNode is null)
        {
            attachedNode = tooltipOwner;
        
            UpdateText(text);

            tooltipOwner->Flags = (short) ((NodeFlags)tooltipOwner->Flags | NodeFlags.RespondToMouse | NodeFlags.EmitsEvents | NodeFlags.HasCollision);
        
            var tooltipInfo = new AtkTooltipManager.AtkTooltipArgs
            {
                Flags = 0xFFFFFFFF,
                Text = (byte*) tooltipMemory,
                Unk_14 = 0,
                Unk_16 = 0,
                TypeSpecificID = 0,
            };
            
            AtkStage.GetSingleton()->TooltipManager.AddTooltip(AtkTooltipManager.AtkTooltipType.Text, parentAddon->ID, tooltipOwner, &tooltipInfo);
        }
        else
        {
            PluginLog.Warning("Attempted to add a tooltip to a node that already has a tooltip. Use UpdateText instead.");
        }
    }

    public void RemoveTooltip()
    {
        if (attachedNode is not null)
        {
            AtkStage.GetSingleton()->TooltipManager.RemoveTooltip(attachedNode);
            attachedNode = null;
        }
    }

    public void UpdateText(string text)
    {
        var encodedString = new SeStringBuilder().AddText(text).Encode();
        
        Marshal.Copy(encodedString, 0, tooltipMemory, encodedString.Length);
        Marshal.WriteByte(tooltipMemory, encodedString.Length, 0);
    }

    public void Dispose()
    {
        Marshal.FreeHGlobal(tooltipMemory);

        if (attachedNode is not null)
        {
            AtkStage.GetSingleton()->TooltipManager.RemoveTooltip(attachedNode);
        }
    }
}