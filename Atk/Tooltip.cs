using System;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Atk;

public unsafe class Tooltip : IDisposable
{
    private SimpleEvent? eventHandlerInfo;

    private Func<string>? getTooltipTextAction;

    public void AddTooltip(AtkUnitBase* parentAddon, AtkResNode* targetNode)
    {
        if (eventHandlerInfo is not null) throw new Exception("Attempted to add event handler to a tooltip that already has an event handler.");

        eventHandlerInfo = new SimpleEvent(TooltipDelegate);
        
        eventHandlerInfo.AddEvent(parentAddon, targetNode, AtkEventType.MouseOver);
        eventHandlerInfo.AddEvent(parentAddon, targetNode, AtkEventType.MouseOut);
    }

    public void RemoveTooltip(AtkUnitBase* parentAddon, AtkResNode* targetNode)
    {
        if (eventHandlerInfo is null) throw new Exception("Attempted to remove event handler from a tooltip that doesn't have an event handler.");
        
        eventHandlerInfo.RemoveEvent(parentAddon, targetNode, AtkEventType.MouseOver);
        eventHandlerInfo.RemoveEvent(parentAddon, targetNode, AtkEventType.MouseOut);
        
        eventHandlerInfo.Dispose();
    }

    public void SetTooltipStringFunction(Func<string> func) => getTooltipTextAction = func;
    
    private void TooltipDelegate(AtkEventType eventType, AtkUnitBase* atkUnitBase, AtkResNode* node)
    {
        switch (eventType)
        {
            case AtkEventType.MouseOver:
                AtkStage.GetSingleton()->TooltipManager.ShowTooltip(atkUnitBase->ID, node, getTooltipTextAction?.Invoke() ?? "GetTooltipTextAction lambda null");
                break;
            
            case AtkEventType.MouseOut:
                AtkStage.GetSingleton()->TooltipManager.HideTooltip(atkUnitBase->ID);
                break;
        }
    }
    
    public void Dispose()
    {
        eventHandlerInfo?.Dispose();
    }
}