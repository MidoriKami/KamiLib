using System.Collections.Generic;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;

namespace KamiLib.Classes;

public interface IComboItem {
	string Label { get; }
	void Draw();
}

public class Combo(string name, List<IComboItem> items) {
	private IComboItem selectedItem = items[0];
	
	public void Draw() {
		if (items.Count != 0) {
			ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
			using (var combo = ImRaii.Combo($"##{name}",selectedItem.Label, ImGuiComboFlags.HeightLarge)) {
				if (combo) {
					foreach (var item in items) {
						if (ImGui.Selectable(item.Label, selectedItem == item)) {
							selectedItem = item;
						}
					}
				}
			}

			using (var child = ImRaii.Child("selectedItem", ImGui.GetContentRegionAvail())) {
				if (child) {
					selectedItem.Draw();
				}
			}
		}
		else {
			ImGui.Text("KamiLib.Combo Error, no items.");
		}
	}
}
