using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Interface.Utility.Raii;
using ImGuiNET;

namespace KamiLib.Window;

public abstract class SelectionListWindow<T>(string windowName, Vector2 size, bool fixedSize = false) : Window(windowName, size, fixedSize) where T : notnull {
    private T? selectedOption;
    
    protected abstract List<T> Options { get; }
    protected abstract float SelectionListWidth { get; set; }
    protected abstract float SelectionItemHeight { get; }
    protected abstract void DrawListOption(T option);
    protected abstract void DrawSelectedOption(T option);
    protected virtual bool AllowChildScrollbar => false;
    protected virtual bool AllowChildScroll => true;
    protected virtual bool ShowListButton => false;

    protected override void DrawContents() {
        using var table = ImRaii.Table("selectionListWindowTable", 2, ImGuiTableFlags.Resizable, ImGui.GetContentRegionAvail());
        if (!table) return;
        
        ImGui.TableSetupColumn("##selectionList", ImGuiTableColumnFlags.WidthFixed, SelectionListWidth * ImGuiHelpers.GlobalScale);
        ImGui.TableSetupColumn("##selectionContents", ImGuiTableColumnFlags.WidthStretch);

        ImGui.TableNextColumn();
        using var frameBg = ImRaii.PushColor(ImGuiCol.FrameBg, ImGui.GetStyle().Colors[(int) ImGuiCol.FrameBg] with { W = 0.10f });
        using var scrollBarSize = ImRaii.PushStyle(ImGuiStyleVar.ScrollbarSize, 0.0f);

        var extraButtonSize = new Vector2(ImGui.GetContentRegionAvail().X, 28.0f * ImGuiHelpers.GlobalScale);
        var listBoxSize = ImGui.GetContentRegionAvail() - ImGui.GetStyle().ItemInnerSpacing;

        if (ShowListButton) {
            listBoxSize -= extraButtonSize;
        }
        
        using (var listBox = ImRaii.ListBox("##selectableListBox", listBoxSize)) {
            if (listBox) {
                using var headerHoverColor = ImRaii.PushColor(ImGuiCol.HeaderHovered, ImGui.GetStyle().Colors[(int) ImGuiCol.HeaderHovered] with { W = 0.1f });
                using var textSelectedColor = ImRaii.PushColor(ImGuiCol.Header, ImGui.GetStyle().Colors[(int) ImGuiCol.Header] with { W = 0.1f });

                ImGuiClip.ClippedDraw(Options, DrawOptionClipped, SelectionItemHeight * ImGuiHelpers.GlobalScale);
            }
        }
        
        if (ShowListButton) {
            using var buttonChild = ImRaii.Child("buttonChild", ImGui.GetContentRegionAvail() - ImGui.GetStyle().ItemInnerSpacing, false, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
            DrawExtraButton();
        }
        
        frameBg.Pop();
        scrollBarSize.Pop();
        
        ImGui.TableNextColumn();
        var flags = ImGuiWindowFlags.None;
        flags |= AllowChildScrollbar ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoScrollbar;
        flags |= AllowChildScroll ? ImGuiWindowFlags.None : ImGuiWindowFlags.NoScrollWithMouse;
        
        using (var _ = ImRaii.Child("selectedOption", ImGui.GetContentRegionAvail() - ImGui.GetStyle().ItemInnerSpacing, false, flags)) {
            if (selectedOption is not null) {
                DrawSelectedOption(selectedOption);
            }
            else {
                const string noSelectionText = "Select an option";
                var textSize = ImGui.CalcTextSize(noSelectionText);
                
                ImGui.SetCursorPos(ImGui.GetContentRegionMax() / 2.0f - textSize / 2.0f);
                ImGui.TextColored(KnownColor.Orange.Vector(), noSelectionText);
            }
        }
    }

    private void DrawOptionClipped(T option) {
        var cursorPosition = ImGui.GetCursorPos();
        if (ImGui.Selectable($"##{option.GetHashCode()}", selectedOption?.Equals(option) ?? false, ImGuiSelectableFlags.None, new Vector2(ImGui.GetContentRegionAvail().X, SelectionItemHeight * ImGuiHelpers.GlobalScale))) {
            selectedOption = option;
        }

        ImGui.SetCursorPos(cursorPosition);
        DrawListOption(option);
    }
    
    protected virtual void DrawExtraButton() { }

    protected void DeselectItem() {
        selectedOption = default;
    }
}