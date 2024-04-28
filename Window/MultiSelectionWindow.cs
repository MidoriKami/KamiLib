using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace KamiLib.Window;

public class MultiSelectionWindow<T> : Window where T : class {
    public MultiSelectionWindow(WindowManager windowManager) : this(windowManager, new Vector2(600.0f, 400.0f)) {
    }

    public MultiSelectionWindow(WindowManager windowManager, Vector2 size) : base($"{typeof(T).Name} Multi-Selection Window", size, true) {
        this.windowManager = windowManager;
        UnCollapseOrShow();
        
        TitleBarButtons.Add(new TitleBarButton {
            Icon = FontAwesomeIcon.Plus,
            ShowTooltip = () => ImGui.SetTooltip("Select All"),
            Click = _ => SelectAllOptions(),
            IconOffset = new Vector2(2.0f, 1.0f),
            Priority = 2,
        });
    }
    
    private readonly WindowManager windowManager;
    
    // We only need to read the collection, but we don't have a usable overload for ClippedDraw that accepts an IEnumerable.
    // ReSharper disable once CollectionNeverUpdated.Global
    public required List<T> SelectionOptions { get; init; }
    
    public required Action<List<T>?> SelectionCallback { get; init; }
    public required Action<T> DrawSelection { get; init; }
    public required float SelectionHeight { get; init; }
    
    private List<T> selected = [];

    public override void Draw() {
        base.Draw();

        DrawResults();
        DrawConfirmCancel();
    }

    public override void OnClose() {
        base.OnClose();
        
        windowManager.RemoveWindow(this);
    }

    private void DrawResults() {
        var sectionSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 40.0f * ImGuiHelpers.GlobalScale);
        
        using var searchResults = ImRaii.Child("resultsChild", sectionSize, false, ImGuiWindowFlags.NoMove);
        if (!searchResults) return;
        
        ImGuiClip.ClippedDraw(SelectionOptions, DrawSelectable, SelectionHeight * ImGuiHelpers.GlobalScale);
    }

    private void DrawConfirmCancel() {
        using var confirmationButtons = ImRaii.Child("selectCancel", ImGui.GetContentRegionAvail());
        if (!confirmationButtons) return;
        
        ImGuiHelpers.ScaledDummy(5.0f);

        using (var _ = ImRaii.Disabled(selected.Count == 0)) {
            if (ImGui.Button("Confirm", ImGuiHelpers.ScaledVector2(100.0f, 25.0f))) {
                SelectionCallback(selected);
                Close();
            }
        }
            
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 100.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Cancel", ImGuiHelpers.ScaledVector2(100.0f, 25.0f))) {
            SelectionCallback(null);
            Close();
        }
    }

    private void DrawSelectable(T selectionOption) {
        using var selectable = ImRaii.Child($"{selectionOption.GetHashCode()}", new Vector2(ImGui.GetContentRegionAvail().X, SelectionHeight * ImGuiHelpers.GlobalScale));
        if (!selectable) return;

        var cursorPosition = ImGui.GetCursorPos();
            
        if (ImGui.Selectable($"##{selectable.GetHashCode()}", selected.Contains(selectionOption), ImGuiSelectableFlags.None, new Vector2(ImGui.GetContentRegionAvail().X, SelectionHeight * ImGuiHelpers.GlobalScale))) {
            if (selected.Contains(selectionOption)) {
                selected = selected.Where(option => option != selectionOption).ToList();
            }
            else {
                selected.Add(selectionOption);
            }
        }
            
        ImGui.SetCursorPos(cursorPosition);

        DrawSelection(selectionOption);
    }

    private void SelectAllOptions() => selected = SelectionOptions;
}