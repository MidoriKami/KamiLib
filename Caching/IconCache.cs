using System;
using System.Collections.Generic;
using ImGuiScene;

namespace KamiLib.Caching;

public class IconCache : IDisposable
{
    private readonly Dictionary<uint, TextureWrap?> iconTextures = new();
    
    private static IconCache? _instance;
    public static IconCache Instance => _instance ??= new IconCache();
    
    public static void Cleanup()
    {
        _instance?.Dispose();
    }
    
    public void Dispose() 
    {
        foreach (var texture in iconTextures.Values) 
        {
            texture?.Dispose();
        }

        iconTextures.Clear();
    }
        
    private void LoadIconTexture(uint iconId) 
    {
        try
        {
            var tex = Service.TextureProvider.GetIcon(iconId);

            if (tex is not null && tex.ImGuiHandle != nint.Zero) 
            {
                iconTextures[iconId] = tex;
            } 
            else 
            {
                tex?.Dispose();
            }
        } 
        catch (Exception ex) 
        {
            Service.Log.Error($"Failed loading texture for icon {iconId} - {ex.Message}");
        }
    }
    
    public TextureWrap? GetIcon(uint iconId) 
    {
        if (iconTextures.TryGetValue(iconId, out var value)) return value;

        iconTextures.Add(iconId, null);
        LoadIconTexture(iconId);

        return iconTextures[iconId];
    }
}