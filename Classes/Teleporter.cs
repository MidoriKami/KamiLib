using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.Sheets;

namespace KamiLib.Classes;

public unsafe class Teleporter {
	[PluginService] private IDataManager DataManager { get; set; } = null!;
	[PluginService] private IChatGui ChatGui { get; set; } = null!;

	private readonly string pluginName;

	public Teleporter(IDalamudPluginInterface pluginInterface) {
		pluginInterface.Inject(this);
		pluginName = pluginInterface.InternalName;
	}
	 
	public void Teleport(uint aetheryteId) {
		var aetheryte = DataManager.GetExcelSheet<Aetheryte>().GetRow(aetheryteId);
		Telepo.Instance()->Teleport(aetheryteId, 0);
		ChatGui.Print(new XivChatEntry {
			Message = new SeStringBuilder()
				.AddUiForeground($"[{pluginName}] ", 45)
				.AddUiForeground("[Teleport] ", 62)
				.AddText("Teleporting to ")
				.AddUiForeground(aetheryte.PlaceName.Value.Name.ExtractText(), 576)
				.Build(),
		});
	}
}