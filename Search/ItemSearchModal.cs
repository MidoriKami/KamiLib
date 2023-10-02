using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Interface.Utility;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Search;

public enum SpecialSearchType
{
    Item,
    HighQualityItem,
    Collectable,
}

public class ItemSearchModal
{
    private Item? selectedItem;
    private List<Item> searchResults = new();
    private string searchString = string.Empty;
    private SpecialSearchType searchMode;

    public required string Name { get; init; }
    public required Action<Item, SpecialSearchType> AddAction { get; init; }

    public void DrawSearchModal()
    {
        ImGui.SetNextWindowSizeConstraints(new Vector2(400.0f, 300.0f), new Vector2(400.0f, 300.0f));
        if (ImGui.BeginPopupModal(Name))
        {
            if (ImGui.IsWindowAppearing())
            {
                ImGui.SetKeyboardFocusHere();
            }
            
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            if (ImGui.InputTextWithHint("##SearchBox", "Search...", ref searchString, 2048, ImGuiInputTextFlags.AutoSelectAll))
            {
                UpdateSearch();
            }

            DrawRadioButtons();
            ImGuiHelpers.ScaledDummy(5.0f);

            var childSize = new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - 23.0f * ImGuiHelpers.GlobalScale - ImGui.GetStyle().FramePadding.Y - 5.0f * ImGuiHelpers.GlobalScale);
            if (ImGui.BeginChild("##SearchResults", childSize))
            {
                ImGuiHelpers.ScaledDummy(2.0f);
            
                if (searchResults is { Count: > 0 })
                {
                    ImGuiClip.ClippedDraw(searchResults, DrawSearchResult, 24.0f);
                }
                else
                {
                    const string text = "No Results";
                    var textSize = ImGui.CalcTextSize(text);
                    ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X / 2.0f - textSize.X / 2.0f);
                    ImGui.SetCursorPosY(ImGui.GetContentRegionAvail().Y / 2.0f - textSize.Y / 2.0f);
                    ImGui.TextColored(KnownColor.OrangeRed.Vector(), text);
                }
            }
            ImGui.EndChild();
            DrawAddCancelButtons();

            ImGui.EndPopup();
        }
    }
    
    private void DrawSearchResult(Item item)
    {
        if (ImGui.Selectable($"##{item.RowId}", selectedItem == item))
        {
            selectedItem = item;
        }

        ImGui.SameLine();
                    
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() - 3.0f * ImGuiHelpers.GlobalScale);
        ImGui.Image(Service.TextureProvider.GetIcon(item.Icon)!.ImGuiHandle, ImGuiHelpers.ScaledVector2(24.0f));
        ImGui.SameLine();
        ImGui.Text(item.Name.RawString);
    }
    
    private void DrawRadioButtons()
    {
        if (ImGui.BeginTable("##RadioButtons", 3, ImGuiTableFlags.SizingStretchProp))
        {
            var intMode = (int) searchMode;

            ImGui.TableNextColumn();
            if (ImGui.RadioButton("Normal Items", ref intMode, (int) SpecialSearchType.Item))
            {
                searchMode = SpecialSearchType.Item;
                UpdateSearch();
            }
            
            ImGui.TableNextColumn();
            if (ImGui.RadioButton("HQ Items", ref intMode, (int) SpecialSearchType.HighQualityItem))
            {
                searchMode = SpecialSearchType.HighQualityItem;
                UpdateSearch();
            }
            
            ImGui.TableNextColumn();
            if (ImGui.RadioButton("Collectable Items", ref intMode, (int) SpecialSearchType.Collectable))
            {
                searchMode = SpecialSearchType.Collectable;
                UpdateSearch();
            }
            
            ImGui.EndTable();
        }
    }
    
    private void UpdateSearch() => searchResults = Service.DataManager.GetExcelSheet<Item>()!
        .Where(item => !item.Name.RawString.IsNullOrEmpty())
        .Where(item => item.Name.ToDalamudString().TextValue.ToLowerInvariant().Contains(searchString.ToLowerInvariant()))
        .Where(item => searchMode switch
        {
            SpecialSearchType.Collectable => item.IsCollectable,
            SpecialSearchType.HighQualityItem => item.CanBeHq,
            _ => !item.AlwaysCollectable
        })
        .OrderBy(item => item.Name.RawString)
        .ToList();

    private void DrawAddCancelButtons()
    {
        ImGui.SetCursorPosY(ImGui.GetContentRegionMax().Y - 23.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Cancel", ImGuiHelpers.ScaledVector2(100.0f, 23.0f)))
        {
            ImGui.CloseCurrentPopup();
        }

        ImGui.SameLine(ImGui.GetContentRegionMax().X - 100.0f * ImGuiHelpers.GlobalScale);
        if (ImGui.Button("Add", ImGuiHelpers.ScaledVector2(100.0f, 23.0f)))
        {
            if (selectedItem is not null)
            {
                AddAction(selectedItem, searchMode);
            }
            else
            {
                Service.Chat.PrintError("No item selected");
            }
            ImGui.CloseCurrentPopup();
        }
    }
}