using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ImGuiNET;
using KamiLib.Extensions;
using Lumina.Excel.Sheets;

namespace KamiLib.Window.SelectionWindows;

public class ContentsNoteSelectionWindow : IconItemSelectionWindow<ContentsNote> {
	[PluginService] private IDataManager DataManager { get; set; } = null!;
	[PluginService] private ITextureProvider TextureProvider { get; set; } = null!;

	public ContentsNoteSelectionWindow(IDalamudPluginInterface pluginInterface) : base(new Vector2(500.0f, 600.0f)) {
		pluginInterface.Inject(this);

		SelectionOptions = DataManager.GetExcelSheet<ContentsNote>()
			.Where(contentNote => contentNote is { RowId: not 0 } and { Icon: not 0 })
			.ToList();
	}
	
	protected override bool AllowMultiSelect => true;
	protected override float SelectionHeight => 64.0f * ImGuiHelpers.GlobalScale;
	protected override float IconWidth => 64.0f * ImGuiHelpers.GlobalScale;

	protected override void DrawIcon(ContentsNote option)
		=> TextureProvider.DrawGameIcon(option.Icon);

	protected override void DrawText(ContentsNote option) {
		ImGui.Text(option.Name.ExtractText());
		ImGui.Spacing();
		ImGuiHelpers.SafeTextColoredWrapped(KnownColor.Gray.Vector().Lighten(0.20f), option.Description.ExtractText());
	}

	protected override IEnumerable<string> GetFilterStrings(ContentsNote option)
		=> [option.Name.ExtractText(), option.Description.ExtractText()];

	protected override string GetElementKey(ContentsNote element)
		=> element.RowId.ToString();
}