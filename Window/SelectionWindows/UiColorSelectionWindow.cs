using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;
using ImGuiNET;
using KamiLib.Extensions;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Window.SelectionWindows;

public class UiColorSelectionWindow: SelectionWindowBase<UIColor> {
	protected override bool AllowMultiSelect => false;
	
	protected override float SelectionHeight => 32.0f * ImGuiHelpers.GlobalScale;
	
	protected override bool ShowFilter => false;

	public UiColorSelectionWindow(IDataManager dataManager) : base(new Vector2(250.0f, 400.0f))
		=> SelectionOptions = dataManager.GetExcelSheet<UIColor>()!.ToList();

	protected override void DrawSelection(UIColor option) {
		var cursorStart = ImGui.GetCursorScreenPos();
		
		ImGui.GetWindowDrawList().AddRectFilled(cursorStart + new Vector2(3.0f, 3.0f), cursorStart + new Vector2(ImGui.GetContentRegionAvail().X, 32.0f * ImGuiHelpers.GlobalScale) - new Vector2(3.0f, 3.0f), ImGui.GetColorU32(option.Foreground()));
		ImGui.SetCursorScreenPos(cursorStart);
		ImGuiHelpers.ScaledDummy(32.0f);
	}

	protected override bool FilterResults(UIColor option, string filter)
		=> true;
}