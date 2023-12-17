using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Events;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiLib.KamiToolKit.Interfaces;

namespace KamiLib.KamiToolKit.Controllers;

public unsafe class ClickHandler : IDisposable {
    private readonly List<IAddonEventHandle?> clickHandles = new();
    private readonly IResNode resNode;
    private readonly AtkUnitBase* addon;
    private Action? internalOnClick;
    
    public ClickHandler(IResNode node, AtkUnitBase* addon) {
        this.addon = addon;
        resNode = node;
    }
    
    public void Dispose() {
        UnregisterClickEvents();
        internalOnClick = null;
    }
    
    public Action? OnClick {
        set {
            if (internalOnClick is null && value is not null) {
                RegisterClickEvents();
                internalOnClick = value;
            }
            else if (internalOnClick is not null && value is null) {
                UnregisterClickEvents();
                internalOnClick = null;
            }
        }
    }

    private void RegisterClickEvents() {
        clickHandles.AddRange(new List<IAddonEventHandle?>
        {
            Service.EventManager.AddEvent((nint) addon, (nint) resNode.ResNode, AddonEventType.MouseOver, HandleOnClick),
            Service.EventManager.AddEvent((nint) addon, (nint) resNode.ResNode, AddonEventType.MouseOut, HandleOnClick),
            Service.EventManager.AddEvent((nint) addon, (nint) resNode.ResNode, AddonEventType.MouseClick, HandleOnClick)
        });

    }

    private void UnregisterClickEvents() {
        foreach (var clickHandle in clickHandles.OfType<IAddonEventHandle>()) {
            Service.EventManager.RemoveEvent(clickHandle);
        }
        clickHandles.Clear();
    }
    
    private void HandleOnClick(AddonEventType atkEventType, IntPtr atkUnitBase, IntPtr atkResNode) {
        if (internalOnClick is not null) {
            switch (atkEventType) {
                case AddonEventType.MouseOver:
                    Service.EventManager.SetCursor(AddonCursorType.Clickable);
                    break;

                case AddonEventType.MouseOut:
                    Service.EventManager.ResetCursor();
                    break;

                case AddonEventType.MouseClick:
                    internalOnClick.Invoke();
                    break;
            }
        }
    }
}