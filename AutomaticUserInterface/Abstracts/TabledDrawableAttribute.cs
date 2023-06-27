using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class TabledDrawableAttribute : DrawableAttribute
{
    protected TabledDrawableAttribute(string? label) : base(label) { }

    protected override void Draw(object obj, MemberInfo field, Action? saveAction = null) => DrawTabled(obj, field, saveAction);

    protected abstract void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null);

    protected abstract void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null);

    private void DrawTabled(object configObject, MemberInfo field, Action? saveAction = null)
    {
        if (ImGui.BeginTable($"##ValueTable{Label}", 2, ImGuiTableFlags.SizingStretchSame))
        {
            ImGui.TableNextColumn();
            ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);

            DrawLeftColumn(configObject, field, saveAction);

            ImGui.TableNextColumn();
            DrawRightColumn(configObject, field, saveAction);

            ImGui.EndTable();
        }
    }
}