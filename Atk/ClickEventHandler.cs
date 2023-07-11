using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Atk;

public unsafe class ClickEventHandler : IDisposable
{
    private SimpleEvent? eventHandlerInfo;
    private bool clickEventEnabled = true;
    private Action? clickAction;
       
    public void AddClickEvent(AtkUnitBase* parentAddon, AtkResNode* targetNode, Action onClickAction)
    {
        if (eventHandlerInfo is not null) throw new Exception("Attempted to add event handler to a clickHandler that already has an event handler.");

        eventHandlerInfo = new SimpleEvent(ClickEventDelegate);
        clickAction = onClickAction;

        eventHandlerInfo.AddEvent(parentAddon, targetNode, AtkEventType.MouseOver);
        eventHandlerInfo.AddEvent(parentAddon, targetNode, AtkEventType.MouseOut);
        eventHandlerInfo.AddEvent(parentAddon, targetNode, AtkEventType.MouseClick);
    }

    public void RemoveClickEvent(AtkUnitBase* parentAddon, AtkResNode* targetNode)
    {
        if (eventHandlerInfo is null) throw new Exception("Attempted to remove event handler from a clickHandler that doesn't have an event handler.");
        
        eventHandlerInfo.RemoveEvent(parentAddon, targetNode, AtkEventType.MouseOver);
        eventHandlerInfo.RemoveEvent(parentAddon, targetNode, AtkEventType.MouseOut);
        eventHandlerInfo.RemoveEvent(parentAddon, targetNode, AtkEventType.MouseOut);
        
        eventHandlerInfo.Dispose();
    }
    
    public void ToggleClickEvent(bool enabled) => clickEventEnabled = enabled;
    
    private void ClickEventDelegate(AtkEventType eventType, AtkUnitBase* atkUnitBase, AtkResNode* node)
    {
        if (!clickEventEnabled) return;
        
        switch (eventType)
        {
            case AtkEventType.MouseOver:
                CursorController.Instance.SetCursor(AtkCursor.CursorType.Clickable);
                break;
            
            case AtkEventType.MouseOut:
                CursorController.Instance.ResetCursor();
                break;
            
            case AtkEventType.MouseClick:
                clickAction?.Invoke();
                break;
        }
    }
    
    public void Dispose()
    {
        eventHandlerInfo?.Dispose();
    }
}