using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Services;
using KamiLib.Classes;
using KamiLib.Extensions;
using Lumina.Excel.Sheets;

namespace KamiLib.DebugWindows;

public class DutyTypeDebugWindow(IDataManager dataManager) : Window.Window("Duty Type Debug Window", new Vector2(500.0f, 500.0f)) {
	private readonly Combo combo = new("DutyCombo", [
		new DutyComboItem("Alliance Raids", dataManager.GetAllianceDuties()),
		new DutyComboItem("Criterion", dataManager.GetCriterionDuties()),
		new DutyComboItem("Extreme", dataManager.GetExtremeDuties()),
		new DutyComboItem("Unreal", dataManager.GetUnrealDuties()),
		new DutyComboItem("Savage", dataManager.GetSavageDuties()),
		new DutyComboItem("Ultimate", dataManager.GetUltimateDuties()),
		new DutyComboItem("Limited Alliance", dataManager.GetLimitedAllianceRaidDuties()),
		new DutyComboItem("Limited Normal Raids", dataManager.GetLimitedNormalRaidDuties()),
		new DutyComboItem("Limited Savage Raids", dataManager.GetLimitedSavageRaidDuties()),
	]);
	
	protected override void DrawContents() {
		combo.Draw();
	}

	private class DutyComboItem(string internalName, IEnumerable<ContentFinderCondition> duties) : IComboItem {
		private IEnumerable<ContentFinderCondition> Duties { get; set; } = duties;
		private string InternalName { get; set; } = internalName;

		public string Label => InternalName;
		public void Draw() {
			if (Duties.Any()) {
				foreach (var entry in Duties) {
					ImGui.Text(entry.Name.ExtractText());
				}
			}
			else {
				ImGui.Text("No results for type");
			}
		}
	}
}
