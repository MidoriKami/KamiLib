using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class TabledDrawableAttribute : DrawableAttribute
{
    protected TabledDrawableAttribute(string? labelLocKey) : base(labelLocKey) { }

    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null) => DrawTabled(obj, field, saveAction);

    protected abstract void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null);

    protected abstract void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null);

    private void DrawTabled(object configObject, FieldInfo field, Action? saveAction = null)
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