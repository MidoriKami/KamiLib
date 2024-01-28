using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Controllers;

public sealed unsafe class TooltipHandler : NativeEventHandler<Func<SeString>> {
    private readonly IResNode resNode;
    private readonly AtkUnitBase* addon;

    public TooltipHandler(IResNode node, AtkUnitBase* addon) {
        this.addon = addon;
        resNode = node;
    }

    public SeString? Text {
        set {
            if (value is null) {
                InternalEvent = null;
            }
            else {
                InternalEvent = () => value;
            }
        }
    }
    
    protected override IEnumerable<IAddonEventHandle?> RegisterEvents() => new List<IAddonEventHandle?> {
        Service.EventManager.AddEvent((nint) addon, (nint) resNode.ResNode, AddonEventType.MouseOver, HandleEvent),
        Service.EventManager.AddEvent((nint) addon, (nint) resNode.ResNode, AddonEventType.MouseOut, HandleEvent)
    };

    private void HandleEvent(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
        if (InternalEvent is not null) {
            switch (atkEventType) {
                case AddonEventType.MouseOver:
                    AtkStage.GetSingleton()->TooltipManager.ShowTooltip(addon->ID, (AtkResNode*) atkResNode, InternalEvent?.Invoke().Encode());
                    break;

                case AddonEventType.MouseOut:
                    AtkStage.GetSingleton()->TooltipManager.HideTooltip(addon->ID);
                    break;
            }
        }
    }
}