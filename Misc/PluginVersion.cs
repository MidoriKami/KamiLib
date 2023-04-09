using System.Diagnostics;
using System.Numerics;
using ImGuiNET;

namespace KamiLib.Misc;


public class PluginVersion
{
    private static PluginVersion? _instance;
    public static PluginVersion Instance => _instance ??= new PluginVersion();

    private readonly string versionText;
    
    private PluginVersion()
    {
        versionText = GetVersionText();
    }
    
    private static string GetVersionText()
    {
        foreach (var frame in new StackTrace().GetFrames())
        {
            var assembly = frame.GetMethod()?.DeclaringType?.Assembly;

            if (assembly?.GetName().Name == KamiCommon.PluginName)
            {
                var assemblyInformation = assembly.FullName!.Split(',');

                return assemblyInformation[1].Replace('=', ' ');
            }
        }

        return "Unable to Read Assembly";
    }
    
    public void DrawVersionText()
    {
        var region = ImGui.GetContentRegionAvail();

        var versionTextSize = ImGui.CalcTextSize(versionText) / 2.0f;
        var cursorStart = ImGui.GetCursorPos();
        cursorStart.X += region.X / 2.0f - versionTextSize.X;

        ImGui.SetCursorPos(cursorStart);
        ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1.0f), versionText);
    }
}