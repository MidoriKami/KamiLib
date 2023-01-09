using System;
using System.Collections.Generic;
using System.Linq;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Logging;
using Dalamud.Plugin.Ipc;
using Dalamud.Plugin.Ipc.Exceptions;
using KamiLib.Caching;
using KamiLib.ChatCommands;
using KamiLib.Localization;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Teleporter;

public class TeleportInfo
{
    public uint CommandID { get; }
    public Enum Target { get; }
    public Aetheryte Aetherite { get; }
    
    public TeleportInfo(uint commandID, Enum target, uint aetheriteID)
    {
        CommandID = commandID;
        Target = target;
        Aetherite = GetAetheryte(aetheriteID);
    }
    
    private static Aetheryte GetAetheryte(uint id) => LuminaCache<Aetheryte>.Instance.GetRow(id)!;
}

public record TeleportLinkPayloads(Enum Location, DalamudLinkPayload Payload);

public class TeleportManager : IDisposable
{
    private static TeleportManager? _instance;
    public static TeleportManager Instance => _instance ??= new TeleportManager();
    
    private readonly ICallGateSubscriber<uint, byte, bool> teleportIpc;
    private readonly ICallGateSubscriber<bool> showChatMessageIpc;

    private readonly List<TeleportInfo> teleportInfoList = new();

    private List<TeleportLinkPayloads> ChatLinkPayloads { get; } = new();

    private TeleportManager()
    {
        teleportIpc = Service.PluginInterface.GetIpcSubscriber<uint, byte, bool>(Strings.Teleport_Label);
        showChatMessageIpc = Service.PluginInterface.GetIpcSubscriber<bool>("Teleport.ChatMessage");
    }

    public static void Cleanup()
    {
        _instance?.Dispose();
    }
    
    public void Dispose()
    {
        foreach (var payload in teleportInfoList)
        {
            Service.PluginInterface.RemoveChatLinkHandler(payload.CommandID);
        }
    }

    public void AddTeleports(IEnumerable<TeleportInfo> teleports)
    {
        teleportInfoList.AddRange(teleports);
        
        foreach (var teleport in teleportInfoList)
        {
            Service.PluginInterface.RemoveChatLinkHandler(teleport.CommandID);

            var linkPayload = Service.PluginInterface.AddChatLinkHandler(teleport.CommandID, TeleportAction);

            ChatLinkPayloads.Add(new TeleportLinkPayloads(teleport.Target, linkPayload));
        }
    }

    private void TeleportAction(uint command, SeString message)
    {
        var teleportInfo = teleportInfoList.First(teleport => teleport.CommandID == command);

        if (AetheryteUnlocked(teleportInfo.Aetherite, out var targetAetheriteEntry))
        {
            Teleport(targetAetheriteEntry!);
        }
        else
        {
            PluginLog.Error("User attempted to teleport to an aetheryte that is not unlocked");
            UserError(Strings.Teleport_NotUnlocked);
        }
    }

    public DalamudLinkPayload GetPayload(Enum targetLocation)
    {
        return ChatLinkPayloads.First(payload => Equals(payload.Location, targetLocation)).Payload;
    }

    private void Teleport(AetheryteEntry aetheryte)
    {
        try
        {
            var didTeleport = teleportIpc.InvokeFunc(aetheryte.AetheryteId, aetheryte.SubIndex);
            var showMessage = showChatMessageIpc.InvokeFunc();

            if (!didTeleport)
            {
                UserError(Strings.Teleport_BadSituation);
            }
            else if (showMessage)
            {
                Chat.Print(Strings.Teleport_Label, string.Format(Strings.Teleport_TeleportingTo, GetAetheryteName(aetheryte)));
            }
        }
        catch (IpcNotReadyError)
        {
            PluginLog.Error("Teleport IPC not found");
            UserError(Strings.Teleport_InstallTeleporter);
        }
    }

    private void UserError(string error)
    {
        Service.Chat.PrintError(error);
        Service.Toast.ShowError(error);
    }

    private string GetAetheryteName(AetheryteEntry aetheryte)
    {
        var gameData = aetheryte.AetheryteData.GameData;
        var placeName = gameData?.PlaceName.Value;

        return placeName == null ? "[Name Lookup Failed]" : placeName.Name;
    }

    private bool AetheryteUnlocked(ExcelRow aetheryte, out AetheryteEntry? entry)
    {
        if (Service.AetheryteList.Any(entry => entry.AetheryteId == aetheryte.RowId))
        {
            entry = Service.AetheryteList.First(entry => entry.AetheryteId == aetheryte.RowId);
            return true;
        }
        else
        {
            entry = null;
            return false;
        }
    }
}
