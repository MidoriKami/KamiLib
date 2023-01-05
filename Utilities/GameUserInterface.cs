using System;
using Dalamud.Game;
using FFXIVClientStructs.FFXIV.Component.GUI;
// ReSharper disable ConditionIsAlwaysTrueOrFalse

namespace KamiLib.Utilities;

public unsafe class GameUserInterface : IDisposable
{
    private bool LastState;
    
    public event EventHandler? UiHidden;
    public event EventHandler? UiShown;

    private static GameUserInterface? _instance;
    public static GameUserInterface Instance => _instance ??= new GameUserInterface();

    public bool IsVisible => !LastState;
    
    private GameUserInterface()
    {
        Service.Framework.Update += FrameworkUpdate;
    }
        
    public static void Cleanup()
    {
        _instance?.Dispose();
    }

    public void Dispose()
    {
        Service.Framework.Update -= FrameworkUpdate;
    }

    private void FrameworkUpdate(Framework framework)
    {
        var partyList = (AtkUnitBase*) Service.GameGui.GetAddonByName("_PartyList", 1);
        var todoList = (AtkUnitBase*) Service.GameGui.GetAddonByName("_ToDoList", 1);
        var enemyList = (AtkUnitBase*) Service.GameGui.GetAddonByName("_EnemyList", 1);

        var partyListVisible = partyList != null && partyList->IsVisible;
        var todoListVisible = todoList != null && todoList->IsVisible;
        var enemyListVisible = enemyList != null && enemyList->IsVisible;

        var shouldHideUi = !partyListVisible && !todoListVisible && !enemyListVisible;

        if (LastState != shouldHideUi)
        {
            if (shouldHideUi)
            {
                UiHidden?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                UiShown?.Invoke(this, EventArgs.Empty);
            }
        }

        LastState = shouldHideUi;
    }
}