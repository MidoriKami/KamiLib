using System;
using System.Collections.Generic;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Controllers;

public sealed unsafe class ClickHandler : NativeEventHandler<Action> {
    public required IResNode ResNode { private get; init; }
    public required AtkUnitBase* ParentAddon { private get; init; }

    protected override IEnumerable<IAddonEventHandle?> RegisterEvents() => new List<IAddonEventHandle?> {
        Service.EventManager.AddEvent((nint) ParentAddon, (nint) ResNode.ResNode, AddonEventType.MouseOver, HandleEvent),
        Service.EventManager.AddEvent((nint) ParentAddon, (nint) ResNode.ResNode, AddonEventType.MouseOut, HandleEvent),
        Service.EventManager.AddEvent((nint) ParentAddon, (nint) ResNode.ResNode, AddonEventType.MouseClick, HandleEvent)
    };

    private void HandleEvent(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
        if (InternalEvent is not null) {
            switch (atkEventType) {
                case AddonEventType.MouseOver:
                    Service.EventManager.SetCursor(AddonCursorType.Clickable);
                    break;

                case AddonEventType.MouseOut:
                    Service.EventManager.ResetCursor();
                    break;

                case AddonEventType.MouseClick:
                    InternalEvent.Invoke();
                    break;
            }
        }
    }
}