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

    public bool AllowMultiSelect { get; init; }
    public Action<List<T>>? MultiSelectionCallback { get; init; }
    public Action<T?>? SingleSelectionCallback { get; init; }
    public required List<T> SelectionOptions { get; init; }
    public required Action<T> DrawSelection { get; init; }
    public required float SelectionHeight { get; init; }
    public Func<T, string, bool>? FilterResults { get; init; }
    
    private List<T>? filteredResults;
    private readonly List<T> selected = [];
    private string searchString = string.Empty;

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
            RefreshSearchResults();
        }
    }
    
    private void DrawResults() {
        var sectionSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 40.0f * ImGuiHelpers.GlobalScale);
        
        using var searchResults = ImRaii.Child("resultsChild", sectionSize, false, ImGuiWindowFlags.NoMove);
        if (!searchResults) return;
        
        if (filteredResults is not null) {
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

        using (var _ = ImRaii.Disabled(selected.Count is 0)) {
            if (ImGui.Button("Confirm", ImGuiHelpers.ScaledVector2(100.0f, 25.0f))) {
                MultiSelectionCallback?.Invoke(selected);
                SingleSelectionCallback?.Invoke(selected.FirstOrDefault());
                Close();
            }
        }
            
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 100.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Cancel", ImGuiHelpers.ScaledVector2(100.0f, 25.0f))) {
            MultiSelectionCallback?.Invoke([]);
            SingleSelectionCallback?.Invoke(null);
            Close();
        }
    }

    private void DrawSelectable(T selectionOption) {
        using var selectable = ImRaii.Child($"{selectionOption.GetHashCode()}", new Vector2(ImGui.GetContentRegionAvail().X, SelectionHeight * ImGuiHelpers.GlobalScale));
        if (!selectable) return;

        var cursorPosition = ImGui.GetCursorPos();
            
        if (ImGui.Selectable($"##{selectable.GetHashCode()}", selected.Contains(selectionOption), ImGuiSelectableFlags.None, new Vector2(ImGui.GetContentRegionAvail().X, SelectionHeight * ImGuiHelpers.GlobalScale))) {
            
            // It was already selected, unselect it.
            if (selected.Contains(selectionOption)) {
                selected.Remove(selectionOption);
                
                RefreshSearchResults();
            }
            else {
                if (AllowMultiSelect) {
                    selected.Add(selectionOption);
                }
                else {
                    selected.Clear();
                    selected.Add(selectionOption);
                }
            }
        }
            
        ImGui.SetCursorPos(cursorPosition);

        DrawSelection(selectionOption);
    }
    
    private void RefreshSearchResults() {
        if (FilterResults is null) return;
        
        filteredResults = SelectionOptions
            .Where(option => FilterResults(option, searchString) && !selected.Contains(option))
            .ToList();
    }
}