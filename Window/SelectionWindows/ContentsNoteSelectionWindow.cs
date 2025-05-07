using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace KamiLib.Window.SelectionWindows;

public class ContentsNoteSelectionWindow : SelectionWindowBase<ContentsNote> {
	[PluginService] private IDataManager DataManager { get; set; } = null!;
	[PluginService] private ITextureProvider TextureProvider { get; set; } = null!;

	public ContentsNoteSelectionWindow(IDalamudPluginInterface pluginInterface) : base(new Vector2(500.0f, 400.0f)) {
		pluginInterface.Inject(this);

		SelectionOptions = DataManager.GetExcelSheet<ContentsNote>()
			.Where(contentNote => contentNote is { RowId: not 0 } and { Icon: not 0 })
			.ToList();
	}
	
	protected override bool AllowMultiSelect => true;
	protected override float SelectionHeight => 64.0f * ImGuiHelpers.GlobalScale;
	
	protected override void DrawSelection(ContentsNote option) {
		using (var imageContainer = ImRaii.Child("image_container", ImGuiHelpers.ScaledVector2(64.0f, ImGui.GetContentRegionAvail().Y), false, ImGuiWindowFlags.NoInputs)) {
			if (!imageContainer) return;

			DrawIcon(option);
		}
		
		ImGui.SameLine();
		
		using (var textContainer = ImRaii.Child("text_container", ImGui.GetContentRegionAvail(), false, ImGuiWindowFlags.NoInputs)) {
			if (!textContainer) return;
			
			DrawText(option);
		}
	}

	private void DrawIcon(ContentsNote option) {
		var icon = TextureProvider.GetFromGameIcon(new GameIconLookup((uint) option.Icon)).GetWrapOrEmpty();
		
		ImGui.Image(icon.ImGuiHandle, icon.Size * ImGuiHelpers.GlobalScale);
	}
	
	private static void DrawText(ContentsNote option) {
		ImGui.Text(option.Name.ExtractText());
		ImGui.Spacing();
		ImGuiHelpers.SafeTextColoredWrapped(KnownColor.Gray.Vector().Lighten(0.20f), option.Description.ExtractText());
	}

	protected override IEnumerable<string> GetFilterStrings(ContentsNote option)
		=> [option.Name.ExtractText(), option.Description.ExtractText()];

	protected override string GetElementKey(ContentsNote element)
		=> element.RowId.ToString();
}