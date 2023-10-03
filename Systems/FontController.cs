using System;
using Dalamud.Interface.GameFonts;

namespace KamiLib.System;

public class FontController : IDisposable
{
    private GameFontHandle? axis12;
    private GameFontHandle? axis18;

    public GameFontHandle Axis12 => axis12 ??= Service.PluginInterface.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Axis12));
    public GameFontHandle Axis18 => axis18 ??= Service.PluginInterface.UiBuilder.GetGameFontHandle(new GameFontStyle(GameFontFamilyAndSize.Axis18));

    public void Dispose()
    {
        axis12?.Dispose();
        axis18?.Dispose();
    }
}