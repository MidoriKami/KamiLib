using System;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Component.GUI;
using CursorType = FFXIVClientStructs.FFXIV.Component.GUI.AtkCursor.CursorType;

namespace KamiLib.Atk;

public unsafe class CursorController : IDisposable
{
    private static CursorController? _instance;
    public static CursorController Instance => _instance ??= new CursorController();
    
    private CursorType currentCursor = CursorType.Arrow;
    private bool cursorSet;

    private delegate nint AtkModuleUpdateCursor(RaptureAtkModule* module);
    
    [Signature("48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 20 4C 8B F1 E8 ?? ?? ?? ?? 49 8B CE", DetourName = nameof(UpdateCursorDetour))]
    private readonly Hook<AtkModuleUpdateCursor>? updateCursorHook = null;
    
    public CursorController()
    {
        SignatureHelper.Initialise(this);
        updateCursorHook?.Enable();
    }
    
    public void SetCursor(CursorType cursor)
    {
        currentCursor = cursor;
        cursorSet = true;
    }

    public void ResetCursor()
    {
        currentCursor = CursorType.Arrow;
        cursorSet = false;
    }
    
    private nint UpdateCursorDetour(RaptureAtkModule* module) 
    {
        if (cursorSet)
        {
            var cursor = AtkStage.GetSingleton()->AtkCursor;
            if (cursor.Type != currentCursor) 
            {
                AtkStage.GetSingleton()->AtkCursor.SetCursorType(currentCursor, 1);
            }
            return nint.Zero;
        }

        return updateCursorHook!.Original(module);
    }

    public static void Cleanup() => _instance?.Dispose();
    
    public void Dispose()
    {
        updateCursorHook?.Dispose();
    }
}