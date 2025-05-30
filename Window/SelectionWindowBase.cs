﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace KamiLib.Window;

public abstract class SelectionWindowBase<T> : Window where T : notnull {
    protected SelectionWindowBase() : this(new Vector2(600.0f, 400.0f)) {
    }

    protected SelectionWindowBase(Vector2 size, bool fixedSize = true, string? alternativeName = null) : base(alternativeName ?? $"{typeof(T).Name} Selection Window", size, fixedSize) {
        TitleBarButtons.Add(new TitleBarButton {
            ShowTooltip = () => {
                ImGui.SetTooltip("Deselect All");
            },
            Icon = FontAwesomeIcon.Minus,
            Click = _ => selected.Clear(),
            IconOffset = new Vector2(2.5f, 1.0f),
        });
        
        TitleBarButtons.Add(new TitleBarButton {
            ShowTooltip = () => {
                ImGui.SetTooltip("Select All");
            },
            Icon = FontAwesomeIcon.Plus,
            Click = _ => {
                if (AllowMultiSelect) {
                    foreach (var option in SelectionOptions.Where(option => !selected.Contains(option))) {
                        selected.Add(option);
                    }
                }
            },
            IconOffset = new Vector2(2.5f, 1.0f),
        });

        UnCollapseOrShow();
    }

    protected abstract bool AllowMultiSelect { get; }
    public Action<List<T>>? MultiSelectionCallback { get; init; }
    public Action<T?>? SingleSelectionCallback { get; init; }
    public List<T> SelectionOptions { get; init; } = [];
    protected abstract float SelectionHeight { get; }
    protected virtual bool ShowFilter => true;

    private List<T>? filteredResults;
    private readonly List<T> selected = [];
    private string searchString = string.Empty;
    private bool resetFocus;
    private DateTime lastSelectionTime = DateTime.MinValue;

    protected abstract void DrawSelection(T option);
    protected abstract IEnumerable<string> GetFilterStrings(T option);

    protected override void DrawContents() {
        TryDrawSearchBox();
        DrawResults();
        DrawConfirmCancel();
    }

    public override void OnClose() {
        base.OnClose();
        
        ParentWindowManager.RemoveWindow(this);
    }

    private void TryDrawSearchBox() {
        if (!ShowFilter) return;
        
        using var searchChild = ImRaii.Child("searchChild", new Vector2(ImGui.GetContentRegionAvail().X, 30.0f * ImGuiHelpers.GlobalScale));
        if (!searchChild) return;
        
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        
        if (ImGui.IsWindowAppearing() || resetFocus) {
            ImGui.SetKeyboardFocusHere();
            resetFocus = false;
        }
        
        if (ImGui.InputTextWithHint("##filterInput", "Search...", ref searchString, 500, ImGuiInputTextFlags.AutoSelectAll)) {
            RefreshSearchResults();
        }

        if ((ImGui.IsKeyPressed(ImGuiKey.Enter) || ImGui.IsKeyPressed(ImGuiKey.KeypadEnter)) && filteredResults is not null && filteredResults.Count > 0) {
            if (!selected.Contains(filteredResults.First())) {
                selected.Add(filteredResults.First());
            }
            else {
                selected.Remove(filteredResults.First());
            }

            resetFocus = true;
        }
    }
    
    private void DrawResults() {
        var sectionSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 40.0f * ImGuiHelpers.GlobalScale);
        
        using var searchResults = ImRaii.Child("resultsChild", sectionSize, false, ImGuiWindowFlags.NoMove);
        if (!searchResults) return;
        
        if (filteredResults is not null) {
            if (filteredResults.Count != 0) {
                ImGuiClip.ClippedDraw(filteredResults, DrawSelectable, SelectionHeight);
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
            ImGuiClip.ClippedDraw(SelectionOptions, DrawSelectable, SelectionHeight);
        }
    }

    private void DrawConfirmCancel() {
        using var confirmationButtons = ImRaii.Child("selectCancel", ImGui.GetContentRegionAvail());
        if (!confirmationButtons) return;
        
        ImGuiHelpers.ScaledDummy(5.0f);

        using (ImRaii.Disabled(selected.Count is 0)) {
            if (ImGui.Button("Confirm", ImGuiHelpers.ScaledVector2(100.0f, 25.0f))) {
                ConfirmSelection();
            }
        }
            
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetContentRegionMax().X - 100.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Cancel", ImGuiHelpers.ScaledVector2(100.0f, 25.0f))) {
            CancelSelection();
        }
    }

    protected abstract string GetElementKey(T element);

    private void DrawSelectable(T selectionOption) {
        var cursorPosition = ImGui.GetCursorPos();
        var selectableSize = new Vector2(ImGui.GetContentRegionAvail().X, SelectionHeight);
        
        using var id = ImRaii.PushId(GetElementKey(selectionOption));
            
        if (ImGui.Selectable("##selectable", selected.Contains(selectionOption), ImGuiSelectableFlags.AllowItemOverlap, selectableSize)) {
            
            // It was already selected
            if (selected.Contains(selectionOption)) {
                
                // And we just tried selecting it again recently, and multiselect is disabled
                if (DateTime.Now - lastSelectionTime < TimeSpan.FromMilliseconds(500) && !AllowMultiSelect) {
                    ConfirmSelection();
                }
                
                // Else, unselect it
                else {
                    selected.Remove(selectionOption);
                    RefreshSearchResults();
                }
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
            
            lastSelectionTime = DateTime.Now;
        }
            
        ImGui.SetCursorPos(cursorPosition);
        using (ImRaii.Child("selection_child", selectableSize, false, ImGuiWindowFlags.NoInputs)) {
            DrawSelection(selectionOption);
        }
    }

    private void RefreshSearchResults() {
        Task.Run(() => {
            filteredResults = SelectionOptions
                .Where(option => GetFilterStrings(option).Any(stringOption => stringOption.Contains(searchString, StringComparison.InvariantCultureIgnoreCase)) || selected.Contains(option))
                .ToList();
        });
    }

    private void ConfirmSelection() {
        MultiSelectionCallback?.Invoke(selected);
        SingleSelectionCallback?.Invoke(selected.FirstOrDefault());
        Close();              
    }

    private void CancelSelection() {
        MultiSelectionCallback?.Invoke([]);
        SingleSelectionCallback?.Invoke(default);
        Close();
    }
}