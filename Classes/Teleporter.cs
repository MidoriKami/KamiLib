using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;

namespace KamiLib.Classes;

public static unsafe class Teleporter {
	public static void Teleport(IDataManager dataManager, IChatGui chatGui, uint aetheryteId, string pluginName) {
		var aetheryte = dataManager.GetExcelSheet<Aetheryte>().GetRow(aetheryteId);
		Telepo.Instance()->Teleport(aetheryteId, 0);
		chatGui.Print(new XivChatEntry {
			Message = new SeStringBuilder()
				.AddUiForeground($"[{pluginName}] ", 45)
				.AddUiForeground("[Teleport] ", 62)
				.AddText("Teleporting to ")
				.AddUiForeground(aetheryte.PlaceName.Value.Name.ExtractText(), 576)
				.Build(),
		});
	}
}