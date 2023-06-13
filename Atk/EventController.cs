using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Atk;

public unsafe class SimpleEvent : IDisposable
{
    public delegate void SimpleEventDelegate(AtkEventType eventType, AtkUnitBase* atkUnitBase, AtkResNode* node);
    
    public SimpleEventDelegate Action { get; init; }
    public uint ParamKey { get; init; }

    public SimpleEvent(SimpleEventDelegate action)
    {
        Action = action;
        ParamKey = GenerateUnusedParamKey();
        
        EventController.Instance.AddHandler(ParamKey, this);
    }
    
    public void Dispose() => EventController.Instance.RemoveHandler(ParamKey);

    private uint GenerateUnusedParamKey()
    {
        // Simple Tweaks uses ranges 0x53540000u -> 0x53550000u
        // We need to make sure to use a different range or else
        // the handlers will trigger each other non-deterministically
        const uint paramStartValue = 0x54550001;

        var newParamKey = paramStartValue;
        while (EventController.Instance.RegisteredParamKeys.Contains(newParamKey))
        {
            if (++newParamKey >= 0x54560000)
            {
                throw new Exception("Too many event handlers.");
            }
        }

        return newParamKey;
    }
    
    public void AddEvent(AtkUnitBase* unitBase, AtkResNode* node, AtkEventType eventType) 
    {
        node->AddEvent(eventType, ParamKey, (AtkEventListener*) unitBase, node, true);
    }

    public void RemoveEvent(AtkUnitBase* unitBase, AtkResNode* node, AtkEventType eventType) 
    {
        node->RemoveEvent(eventType, ParamKey, (AtkEventListener*) unitBase, true);
    }
}

public unsafe class EventController : IDisposable
{
    private static EventController? _instance;
    public static EventController Instance => _instance ??= new EventController();

    private delegate nint GlobalEventHandlerDelegate(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkResNode** eventData, uint* unknown);

    [Signature("48 89 5C 24 ?? 48 89 7C 24 ?? 55 41 56 41 57 48 8B EC 48 83 EC 50 44 0F B7 F2", DetourName = nameof(GlobalEventHandler))]
    private readonly Hook<GlobalEventHandlerDelegate>? globalEventHandlerHook = null;

    private static readonly Dictionary<uint, SimpleEvent> EventHandlers = new();

    public IEnumerable<uint> RegisteredParamKeys => EventHandlers.Keys;

    private EventController()
    {
        SignatureHelper.Initialise(this);
        
        globalEventHandlerHook?.Enable();
    }
    
    public void Dispose()
    {
        globalEventHandlerHook?.Dispose();

        foreach (var handlers in EventHandlers)
        {
            handlers.Value.Dispose();
        }
    }
    
    private nint GlobalEventHandler(AtkUnitBase* atkUnitBase, AtkEventType eventType, uint eventParam, AtkResNode** eventData, uint* unknown)
    {
        if (EventHandlers.TryGetValue(eventParam, out var handler))
        {
            try
            {
                handler.Action.Invoke(eventType, atkUnitBase, eventData[0]);
                return nint.Zero;
            }
            catch (Exception exception)
            {
                PluginLog.Error(exception, "Failed to Execute Custom Event Delegate");
            }
        }

        return globalEventHandlerHook!.Original(atkUnitBase, eventType, eventParam, eventData, unknown);
    }

    public static void Cleanup() => Instance.Dispose();
    
    public void AddHandler(uint paramKey, SimpleEvent eventInfo) => EventHandlers.Add(paramKey, eventInfo);
    public void RemoveHandler(uint paramKey) => EventHandlers.Remove(paramKey);
}