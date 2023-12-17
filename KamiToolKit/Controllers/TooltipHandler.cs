using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Controllers;

public unsafe class TooltipHandler : IDisposable {
    private readonly List<IAddonEventHandle?> tooltipHandles = new();
    private readonly IResNode resNode;
    private readonly AtkUnitBase* addon;
    
    private SeString? internalString;

    public TooltipHandler(IResNode node, AtkUnitBase* addon) {
        this.addon = addon;
        resNode = node;
    }

    public void Dispose() {
        UnregisterTooltipEvents();
        internalString = null;
    }

    public SeString? Text {
        set {
            if (internalString is null && value is not null) {
                RegisterTooltipEvents();
                internalString = value;
            }
            else if (internalString is not null && value is null) {
                UnregisterTooltipEvents();
                internalString = null;
            }
        }
    }

    private void RegisterTooltipEvents() {
        tooltipHandles.AddRange(new List<IAddonEventHandle?>
        {
            Service.EventManager.AddEvent((nint) addon, (nint) resNode.ResNode, AddonEventType.MouseOver, HandleTooltip),
            Service.EventManager.AddEvent((nint) addon, (nint) resNode.ResNode, AddonEventType.MouseOut, HandleTooltip)
        });
    }

    private void UnregisterTooltipEvents() {
        foreach (var tooltipHandle in tooltipHandles.OfType<IAddonEventHandle>()) {
            Service.EventManager.RemoveEvent(tooltipHandle);
        }
        tooltipHandles.Clear();
    }
    
    private void HandleTooltip(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
        var node = (AtkResNode*) atkResNode;

        if (internalString is not null) {
            switch (atkEventType) {
                case AddonEventType.MouseOver:
                    AtkStage.GetSingleton()->TooltipManager.ShowTooltip(addon->ID, node, internalString.Encode());
                    break;

                case AddonEventType.MouseOut:
                    AtkStage.GetSingleton()->TooltipManager.HideTooltip(addon->ID);
                    break;
            }
        }
    }
}