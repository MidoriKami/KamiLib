using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace KamiLib.Window;

public class SelectionWindow<T> : Window where T : class {
    public SelectionWindow(WindowManager windowManager) : this(windowManager, new Vector2(600.0f, 400.0f)) {
    }

    public SelectionWindow(WindowManager windowManager, Vector2 size) : base($"{typeof(T).Name} Selection Window", size, true) {
        this.windowManager = windowManager;
        UnCollapseOrShow();
    }

    private readonly WindowManager windowManager;
    
    public required Action<T?> SelectionCallback { get; init; }
    public required List<T> SelectionOptions { get; init; }
    public required Action<T> DrawSelection { get; init; }
    public required float SelectionHeight { get; init; }
    public Func<T, string, bool>? FilterResults { get; init; }
    
    private List<T>? filteredResults;
    private string searchString = string.Empty;
    private T? selected;

    public override void Draw() {
        base.Draw();

        TryDrawSearchBox();
        DrawResults();
        DrawConfirmCancel();
    }

    public override void OnClose() {
        base.OnClose();
        
        windowManager.RemoveWindow(this);
    }

    private void TryDrawSearchBox() {
        if (FilterResults is null) return;
        
        using var searchChild = ImRaii.Child("searchChild", new Vector2(ImGui.GetContentRegionAvail().X, 30.0f * ImGuiHelpers.GlobalScale));
        if (!searchChild) return;
        
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputTextWithHint("##filterInput", "Search...", ref searchString, 500)) {
            filteredResults = SelectionOptions.Where(option => FilterResults(option, searchString)).ToList();
        }
    }

    private void DrawResults() {
        var sectionSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 40.0f * ImGuiHelpers.GlobalScale);
        
        using var searchResults = ImRaii.Child("resultsChild", sectionSize, false, ImGuiWindowFlags.NoMove);
        if (!searchResults) return;
        
        if (filteredResults is not null) {
            if (selected != null && !filteredResults.Contains(selected)) {
                filteredResults = filteredResults.Prepend(selected).ToList();
            }

            if (filteredResults.Count != 0) {
                ImGuiClip.ClippedDraw(filteredResults, DrawSelectable, SelectionHeight * ImGuiHelpers.GlobalScale);
            }
            else {
                var position = ImGui.GetContentRegionAvail() / 2.0f;
                const string text = "No Results";
                var textSize = ImGui.CalcTextSize(text);
                
                using var textColor = new ImRaii.Color().Push(ImGuiCol.Text, KnownColor.Orange.Vector());
                ImGui.SetCursorPos(position - textSize / 2.0f);
                ImGui.TextUnformatted(text);
            }
        }
        else {
            ImGuiClip.ClippedDraw(SelectionOptions, DrawSelectable, SelectionHeight * ImGuiHelpers.GlobalScale);
        }
    }

    private void DrawConfirmCancel() {
        using var confirmationButtons = ImRaii.Child("selectCancel", ImGui.GetContentRegionAvail());
        if (!confirmationButtons) return;
        
        ImGuiHelpers.ScaledDummy(5.0f);

        using (var _ = ImRaii.Disabled(selected is null)) {
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
            
        if (ImGui.Selectable($"##{selectable.GetHashCode()}", selectionOption == selected, ImGuiSelectableFlags.None, new Vector2(ImGui.GetContentRegionAvail().X, SelectionHeight * ImGuiHelpers.GlobalScale))) {
            if (selectionOption == selected) {
                selected = null;
                
                // Refresh filters.
                if (FilterResults is not null) {
                    filteredResults = SelectionOptions.Where(option => FilterResults(option, searchString)).ToList();
                }
            }
            else {
                selected = selectionOption;
            }
        }
            
        ImGui.SetCursorPos(cursorPosition);

        DrawSelection(selectionOption);
    }
}