using System.Collections.Generic;
using Dalamud.Interface.Utility;
using Dalamud.Plugin.Services;
using KamiLib.Configuration;

namespace KamiLib.Window.SelectionWindows;

internal class CharacterSelectionWindow(bool multiSelect, ITextureProvider textureProvider) : SelectionWindowBase<CharacterConfiguration> {
    protected override bool AllowMultiSelect => multiSelect;
    protected override float SelectionHeight => 75.0f * ImGuiHelpers.GlobalScale;
    
    protected override void DrawSelection(CharacterConfiguration character) 
        => character.Draw(textureProvider);

    protected override IEnumerable<string> GetFilterStrings(CharacterConfiguration option)
        => [option.CharacterName, option.CharacterWorld];
    
    protected override string GetElementKey(CharacterConfiguration element)
        => element.ContentId.ToString();
}