using System;
using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Logging;
using Dalamud.Utility.Signatures;

namespace KamiLib.Utilities;

public unsafe class DutyState : IDisposable
{
    private delegate byte DutyEventDelegate(void* a1, void* a2, ushort* a3);
    [Signature("48 89 5C 24 ?? 48 89 74 24 ?? 57 48 83 EC ?? 48 8B D9 49 8B F8 41 0F B7 08", DetourName = nameof(DutyEventFunction))]
    private readonly Hook<DutyEventDelegate>? DutyEventHook = null;

    public bool IsDutyStarted { get; private set; }

    public event EventHandler? DutyStarted;
    public event EventHandler? DutyWiped;
    public event EventHandler? DutyRecommenced;
    public event EventHandler? DutyCompleted;
    
    private bool CompletedThisTerritory;

    private static DutyState? _instance;
    public static DutyState Instance => _instance ??= new DutyState();

    public DutyState()
    {
        SignatureHelper.Initialise(this);

        DutyEventHook?.Enable();

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
        DutyEventHook?.Dispose();

        Service.Framework.Update -= FrameworkUpdate;
        Service.ClientState.TerritoryChanged -= TerritoryChanged;
    }

    private void FrameworkUpdate(Framework framework)
    {
        if (!IsDutyStarted && !CompletedThisTerritory)
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
            
        CompletedThisTerritory = false;
    }

    private byte DutyEventFunction(void* a1, void* a2, ushort* a3)
    {
        try
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
                        DutyStarted?.Invoke(this, EventArgs.Empty);
                        break;

                    // Party Wipe
                    case 0x40000005:
                        IsDutyStarted = false;
                        DutyWiped?.Invoke(this, EventArgs.Empty);
                        break;

                    // Duty Recommence
                    case 0x40000006:
                        IsDutyStarted = true;
                        DutyRecommenced?.Invoke(this, EventArgs.Empty);
                        break;

                    // Duty Completed
                    case 0x40000003:
                        IsDutyStarted = false;
                        CompletedThisTerritory = true;
                        DutyCompleted?.Invoke(this, EventArgs.Empty);
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Failed to get Duty Started Status");
        }

        return DutyEventHook!.Original(a1, a2, a3);
    }
}