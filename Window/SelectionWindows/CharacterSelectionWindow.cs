using System;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;
using KamiLib.Configuration;

namespace KamiLib.Window.SelectionWindows;

internal class CharacterSelectionWindow(bool multiSelect, ITextureProvider textureProvider) : SelectionWindowBase<CharacterConfiguration> {
    protected override bool AllowMultiSelect => multiSelect;
    protected override float SelectionHeight => 75.0f * ImGuiHelpers.GlobalScale;
    
    protected override void DrawSelection(CharacterConfiguration character) 
        => character.Draw(textureProvider);

    protected override bool FilterResults(CharacterConfiguration option, string filter) 
        => option.CharacterName.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
           option.CharacterWorld.Contains(filter, StringComparison.OrdinalIgnoreCase);
}