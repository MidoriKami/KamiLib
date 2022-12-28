using System.IO;
using ImGuiScene;

namespace KamiLib.Image;

public static class Image
{
    public static TextureWrap LoadImage(string imageName)
    {
        var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
        var imagePath = Path.Combine(assemblyLocation, $@"images\{imageName}.png");

        return Service.PluginInterface.UiBuilder.LoadImage(imagePath);
    }
}