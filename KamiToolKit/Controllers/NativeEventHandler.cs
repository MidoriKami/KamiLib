using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.Addon.Events;

namespace KamiLib.KamiToolKit.Controllers;

public abstract class NativeEventHandler<T> : IDisposable where T : Delegate {
    private readonly List<IAddonEventHandle?> eventHandles = new();
    protected T? InternalEvent;

    public virtual void Dispose() {
        UnregisterEvents();
        InternalEvent = null;
    }

    public T? OnEvent {
        set {
            if (InternalEvent is null && value is not null) {
                eventHandles.AddRange(RegisterEvents());
                InternalEvent = value;
            } else if (InternalEvent is not null && value is null) {
                UnregisterEvents();
                InternalEvent = null;
            }
        }
    }

    protected abstract IEnumerable<IAddonEventHandle?> RegisterEvents();

    private void UnregisterEvents() {
        foreach (var handle in eventHandles.OfType<IAddonEventHandle>()) {
            Service.EventManager.RemoveEvent(handle);
        }
        eventHandles.Clear();
    }
}