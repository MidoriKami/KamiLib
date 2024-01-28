using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.Text.SeStringHandling;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Controllers;

public sealed unsafe class TooltipHandler : NativeEventHandler<Func<SeString>> {
    public required IResNode ResNode { private get; init; }
    public required AtkUnitBase* ParentAddon { private get; init; }

    public SeString? Text {
        set {
            if (value is null) {
                OnEvent = null;
            }
            else {
                OnEvent = () => value;
            }
        }
    }
    
    protected override IEnumerable<IAddonEventHandle?> RegisterEvents() => new List<IAddonEventHandle?> {
        Service.EventManager.AddEvent((nint) ParentAddon, (nint) ResNode.ResNode, AddonEventType.MouseOver, HandleEvent),
        Service.EventManager.AddEvent((nint) ParentAddon, (nint) ResNode.ResNode, AddonEventType.MouseOut, HandleEvent)
    };

    private void HandleEvent(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
        if (InternalEvent is not null) {
            switch (atkEventType) {
                case AddonEventType.MouseOver:
                    AtkStage.GetSingleton()->TooltipManager.ShowTooltip(ParentAddon->ID, (AtkResNode*) atkResNode, InternalEvent?.Invoke().Encode());
                    break;

                case AddonEventType.MouseOut:
                    AtkStage.GetSingleton()->TooltipManager.HideTooltip(ParentAddon->ID);
                    break;
            }
        }
    }
}