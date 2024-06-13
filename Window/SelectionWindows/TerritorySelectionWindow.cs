using System;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiLib.Extensions;
using TerritoryType = Lumina.Excel.GeneratedSheets.TerritoryType;

namespace KamiLib.Window.SelectionWindows;

public class TerritorySelectionWindow : SelectionWindowBase<TerritoryType> {
    [PluginService] private IDataManager DataManager { get; set; } = null!;
    [PluginService] private ITextureProvider TextureProvider { get; set; } = null!;

    public TerritorySelectionWindow(DalamudPluginInterface pluginInterface) :base(new Vector2(600.0f, 600.0f)) {
        pluginInterface.Inject(this);

        SelectionOptions = DataManager.GetExcelSheet<TerritoryType>()!
            .Where(territory => territory is { PlaceName.Row: not 0, LoadingImage: not 0 })
            .ToList();
    }

    protected override bool AllowMultiSelect => true;
    protected override float SelectionHeight => 75.0f * ImGuiHelpers.GlobalScale;
    
    protected override void DrawSelection(TerritoryType option)
        => option.Draw(DataManager, TextureProvider);
   
    protected override bool FilterResults(TerritoryType territory, string filter)
        => territory.PlaceName?.Value?.Name.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase) ?? false;
}