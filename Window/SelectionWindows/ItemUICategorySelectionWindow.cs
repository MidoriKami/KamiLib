using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Window.SelectionWindows;

public class ItemUiCategorySelectionWindow : SelectionWindowBase<ItemUICategory> {
    [PluginService] private ITextureProvider TextureProvider { get; set; } = null!;
    [PluginService] private IDataManager DataManager { get; set; } = null!;

    public ItemUiCategorySelectionWindow(IDalamudPluginInterface pluginInterface) : base(new Vector2(300.0f, 600.0f), false) {
        pluginInterface.Inject(this);

        SelectionOptions = DataManager
            .GetExcelSheet<ItemUICategory>()!
            .Where(item => !item.Name.ToString().IsNullOrEmpty())
            .OrderBy(item => item.OrderMajor)
            .ThenBy(item => item.OrderMinor)
            .ToList();
    }
    
    protected override bool AllowMultiSelect => true;
    protected override float SelectionHeight => 30.0f * ImGuiHelpers.GlobalScale;
    
    protected override void DrawSelection(ItemUICategory option) {
        if (TextureProvider.GetFromGameIcon((uint)option.Icon) is { } texture) {
            ImGui.Image(texture.GetWrapOrEmpty().ImGuiHandle, ImGuiHelpers.ScaledVector2(30.0f, 30.0f));
            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPos().Y + 5.0f * ImGuiHelpers.GlobalScale);
            ImGui.Text(option.Name);
        }
    }

    protected override IEnumerable<string> GetFilterStrings(ItemUICategory option)
        => [option.Name];
}