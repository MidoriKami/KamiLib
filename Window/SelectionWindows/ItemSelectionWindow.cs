using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using ImGuiNET;
using Lumina.Excel.Sheets;

namespace KamiLib.Window.SelectionWindows;

public class ItemSelectionWindow : SelectionWindowBase<Item> {
    [PluginService] private ITextureProvider TextureProvider { get; set; } = null!;
    [PluginService] private IDataManager DataManager { get; set; } = null!;

    public ItemSelectionWindow(IDalamudPluginInterface pluginInterface) : base(new Vector2(300.0f, 600.0f), false) {
        pluginInterface.Inject(this);

        SelectionOptions = DataManager
            .GetExcelSheet<Item>()
            .Where(item => !item.Name.ToString().IsNullOrEmpty())
            .OrderBy(item => item.Name.ToString())
            .ToList();
    }
    
    protected override bool AllowMultiSelect => true;
    protected override float SelectionHeight => 30.0f * ImGuiHelpers.GlobalScale;
    
    protected override void DrawSelection(Item option) {
        if (TextureProvider.GetFromGameIcon(new GameIconLookup {
                IconId = option.Icon,
                ItemHq = false,
                HiRes = true,
            }) is { } texture) {
            ImGui.Image(texture.GetWrapOrEmpty().ImGuiHandle, ImGuiHelpers.ScaledVector2(30.0f, 30.0f));
            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPos().Y + 5.0f * ImGuiHelpers.GlobalScale);
            ImGui.Text(option.Name.ExtractText());
        }
    }

    protected override IEnumerable<string> GetFilterStrings(Item option)
        => [option.Name.ExtractText()];
}