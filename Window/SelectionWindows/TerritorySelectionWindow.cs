﻿using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Interface.Utility;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using KamiLib.Extensions;
using Lumina.Excel.Sheets;

namespace KamiLib.Window.SelectionWindows;

public class TerritorySelectionWindow : SelectionWindowBase<TerritoryType> {
    [PluginService] private IDataManager DataManager { get; set; } = null!;
    [PluginService] private ITextureProvider TextureProvider { get; set; } = null!;

    public TerritorySelectionWindow(IDalamudPluginInterface pluginInterface) :base(new Vector2(600.0f, 600.0f)) {
        pluginInterface.Inject(this);

        SelectionOptions = DataManager.GetExcelSheet<TerritoryType>()
            .Where(territory => territory is { PlaceName.RowId: not 0, LoadingImage.RowId: not 0 })
            .ToList();
    }

    protected override bool AllowMultiSelect => true;
    protected override float SelectionHeight => 75.0f * ImGuiHelpers.GlobalScale;
    
    protected override void DrawSelection(TerritoryType option)
        => option.Draw(DataManager, TextureProvider);

    protected override IEnumerable<string> GetFilterStrings(TerritoryType option)
        => [option.PlaceName.Value.Name.ExtractText()];
    
    protected override string GetElementKey(TerritoryType element)
        => element.RowId.ToString();

}