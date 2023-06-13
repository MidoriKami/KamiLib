using System;
using System.Runtime.InteropServices;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace KamiLib.Atk;

[StructLayout(LayoutKind.Explicit, Size = 0x18)]
public unsafe struct AtkTooltipArgs
{
    [FieldOffset(0x0)] public byte* Text;
    [FieldOffset(0x8)] public ulong TypeSpecificID;
    [FieldOffset(0x10)] public uint Flags;
    [FieldOffset(0x14)] public short Unk_14;
    [FieldOffset(0x16)] public byte Unk_16;
}

// Temporary Class to Implement functions while
// waiting for a ClientStructs merge into Dalamud
public unsafe class TooltipCompat
{
    private delegate void AtkToolTipArgsCtorDelegate(AtkTooltipArgs* structInstance);
    private delegate void ShowTooltipDelegate(AtkTooltipManager* manager, AtkTooltipManager.AtkTooltipType type, ushort parentId, AtkResNode* targetNode, AtkTooltipArgs* tooltipArgs, delegate* unmanaged[Stdcall] <float*, float*, void*> unkDelegate = null, bool unk7 = false, bool unk8 = true);
    private delegate void HideTooltipDelegate(AtkTooltipManager* manager, ushort parentId, bool unknown = false);
    
    [Signature("E8 ?? ?? ?? ?? 48 C7 C2")]
    private readonly AtkToolTipArgsCtorDelegate? atkTooltipArgsCtor = null;

    [Signature("E8 ?? ?? ?? ?? EB 24 66 83 FA 22")]
    private readonly ShowTooltipDelegate? showTooltipInternal = null;

    [Signature("E8 ?? ?? ?? ?? 41 F6 C5 20")]
    private readonly HideTooltipDelegate? hideTooltipInternal = null;
    
    public TooltipCompat()
    {
        SignatureHelper.Initialise(this);
    }

    private void ShowTooltip(ushort parentId, AtkResNode* targetNode, byte* tooltipString)
    {
        var args = stackalloc AtkTooltipArgs[1];
        atkTooltipArgsCtor?.Invoke(args);
        args->Text = tooltipString;
        showTooltipInternal?.Invoke(&AtkStage.GetSingleton()->TooltipManager ,AtkTooltipManager.AtkTooltipType.Text, parentId, targetNode, args);
    }

    public void ShowTooltip(ushort parentId, AtkResNode* targetNode, string tooltipString)
    {
        var utf8StringTooltipLength = System.Text.Encoding.UTF8.GetByteCount(tooltipString);
        var tooltipStringBytes = utf8StringTooltipLength <= 512 ? stackalloc byte[utf8StringTooltipLength + 1] : new byte[utf8StringTooltipLength + 1];
        System.Text.Encoding.UTF8.GetBytes(tooltipString, tooltipStringBytes);
        tooltipStringBytes[utf8StringTooltipLength] = 0;

        fixed (byte* stringPointer = tooltipStringBytes)
        {
            ShowTooltip(parentId, targetNode, stringPointer);
        }
    }

    public void HideTooltip(ushort parentId)
    {
        hideTooltipInternal?.Invoke(&AtkStage.GetSingleton()->TooltipManager, parentId);
    }
}

public unsafe class Tooltip : IDisposable
{
    private readonly TooltipCompat tooltipCompat = new();

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
                tooltipCompat.ShowTooltip(atkUnitBase->ID, node, getTooltipTextAction?.Invoke() ?? "GetTooltipTextAction lambda null");
                break;
            
            case AtkEventType.MouseOut:
                tooltipCompat.HideTooltip(atkUnitBase->ID);
                break;
        }
    }
    
    public void Dispose()
    {
        eventHandlerInfo?.Dispose();
    }
}