using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Lumina.Excel.Sheets;

namespace KamiLib.Window.SelectionWindows;

public class ContentFinderConditionSelectionWindow : SelectionWindowBase<ContentFinderCondition> {

	[PluginService] private IDataManager DataManager { get; set; } = null!;
	[PluginService] private ITextureProvider TextureProvider { get; set; } = null!;

	public ContentFinderConditionSelectionWindow(IDalamudPluginInterface pluginInterface) : base(new Vector2(600.0f, 600.0f)) {
		pluginInterface.Inject(this);

		SelectionOptions = DataManager.GetExcelSheet<ContentFinderCondition>()
			.Where(cfc => cfc is { RowId: not 0, Image: not 0 })
			.ToList();
	}
	
	protected override bool AllowMultiSelect => false;

	protected override float SelectionHeight => 75.0f * ImGuiHelpers.GlobalScale;
	
	private const float Width = 133.5f;
	private const float Height = 75.0f;
	
	protected override void DrawSelection(ContentFinderCondition option) {
		DrawImage(option);
		
		ImGui.SameLine();
		
		DrawInfo(option);
	}

	private void DrawInfo(ContentFinderCondition option) {
		using var contentsFrame = ImRaii.Child("contents_frame", new Vector2(ImGui.GetContentRegionAvail().X, Height * ImGuiHelpers.GlobalScale), false, ImGuiWindowFlags.NoInputs);
		if (!contentsFrame) return;
		
		ImGuiHelpers.ScaledDummy(1.0f);
        
		using var table = ImRaii.Table("data_table", 2, ImGuiTableFlags.SizingStretchProp);
		if (!table) return;
        
		ImGui.TableSetupColumn("##column1", ImGuiTableColumnFlags.None, 3.0f);
		ImGui.TableSetupColumn("##column2", ImGuiTableColumnFlags.None, 1.0f);
		
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(option.Name.ExtractText()));
        
		ImGui.TableNextColumn();
		ImGui.TextUnformatted(option.RowId.ToString());

		ImGui.TableNextColumn();
		ImGui.TextColored(KnownColor.Gray.Vector(), option.TerritoryType.Value.PlaceNameZone.Value.Name.ExtractText());
		
		ImGui.TableNextColumn();
		ImGui.Text(option.ContentType.Value.Name.ExtractText());
	}

	private void DrawImage(ContentFinderCondition option) {
		using var imageFrame = ImRaii.Child("image_frame", ImGuiHelpers.ScaledVector2(Width * ImGuiHelpers.GlobalScale, Height));
		if (!imageFrame) return;

		ImGui.SetCursorPosY(1.0f);
		var image = TextureProvider.GetFromGameIcon(new GameIconLookup { IconId = option.Image }).GetWrapOrEmpty();
		ImGui.Image(image.Handle, ImGuiHelpers.ScaledVector2(Width, Height - 1.0f));
	}

	protected override IEnumerable<string> GetFilterStrings(ContentFinderCondition option)
		=> [ option.Name.ExtractText(), option.ContentType.Value.Name.ExtractText() ];

	protected override string GetElementKey(ContentFinderCondition element)
		=> element.RowId.ToString();
}