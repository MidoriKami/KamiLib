using System.Diagnostics;
using System.Numerics;
using ImGuiNET;

namespace KamiLib.Utility;

public class PluginVersion
{
    private static PluginVersion? _instance;

    private PluginVersion()
    {
        VersionText = GetVersionText();
    }
    public static PluginVersion Instance => _instance ??= new PluginVersion();

    public string VersionText { get; }

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

        var versionTextSize = ImGui.CalcTextSize(VersionText) / 2.0f;
        var cursorStart = ImGui.GetCursorPos();
        cursorStart.X += region.X / 2.0f - versionTextSize.X;

        ImGui.SetCursorPos(cursorStart);
        ImGui.TextColored(new Vector4(0.6f, 0.6f, 0.6f, 1.0f), VersionText);
    }
}