using System;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Utility.Signatures;
using KamiLib.ExceptionSafety;

namespace KamiLib.Utilities;

public unsafe class DutyState : IDisposable
{
    private delegate byte DutyEventDelegate(void* a1, void* a2, ushort* a3);
    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 49 8B F8 41 0F B7 08", DetourName = nameof(DutyEventFunction))]
    private readonly Hook<DutyEventDelegate>? dutyEventHook = null;

    public bool IsDutyStarted { get; private set; }

    public delegate void DutyStateEvent(uint duty);
    
    public event DutyStateEvent? DutyStarted;
    public event DutyStateEvent? DutyWiped;
    public event DutyStateEvent? DutyRecommenced;
    public event DutyStateEvent? DutyCompleted;
    
    private bool completedThisTerritory;

    private static DutyState? _instance;
    public static DutyState Instance => _instance ??= new DutyState();

    private DutyState()
    {
        SignatureHelper.Initialise(this);

        dutyEventHook?.Enable();

        if (Condition.IsBoundByDuty())
        {
            IsDutyStarted = true;
        }

        Service.Framework.Update += FrameworkUpdate;
        Service.ClientState.TerritoryChanged += TerritoryChanged;
    }

    public static void Cleanup()
    {
        _instance?.Dispose();
    }

    public void Dispose()
    {
        dutyEventHook?.Dispose();

        Service.Framework.Update -= FrameworkUpdate;
        Service.ClientState.TerritoryChanged -= TerritoryChanged;
    }

    private void FrameworkUpdate(Framework framework)
    {
        if (!IsDutyStarted && !completedThisTerritory)
        {
            if (Condition.IsBoundByDuty() && Condition.IsInCombat())
            {
                IsDutyStarted = true;
            }
        }
        else if (!Condition.IsBoundByDuty())
        {
            IsDutyStarted = false;
        }
    }

    private void TerritoryChanged(object? sender, ushort e)
    {
        if (IsDutyStarted)
        {
            IsDutyStarted = false;
        }
            
        completedThisTerritory = false;
    }

    private byte DutyEventFunction(void* a1, void* a2, ushort* a3)
    {
        Safety.ExecuteSafe(() =>
        {
            var category = *(a3);
            var type = *(uint*)(a3 + 4);

            // DirectorUpdate Category
            if (category == 0x6D)
            {
                switch (type)
                {
                    // Duty Commenced
                    case 0x40000001:
                        IsDutyStarted = true;
                        DutyStarted?.Invoke(Service.ClientState.TerritoryType);
                        break;

                    // Party Wipe
                    case 0x40000005:
                        IsDutyStarted = false;
                        DutyWiped?.Invoke(Service.ClientState.TerritoryType);
                        break;

                    // Duty Recommence
                    case 0x40000006:
                        IsDutyStarted = true;
                        DutyRecommenced?.Invoke(Service.ClientState.TerritoryType);
                        break;

                    // Duty Completed
                    case 0x40000003:
                        IsDutyStarted = false;
                        completedThisTerritory = true;
                        DutyCompleted?.Invoke(Service.ClientState.TerritoryType);
                        break;
                }
            } 
        });
        
        return dutyEventHook!.Original(a1, a2, a3);
    }
}