using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class RightLabeledTabledDrawableAttribute : TabledDrawableAttribute
{
    protected RightLabeledTabledDrawableAttribute(string? label, string category, int group) : base(label, category, group) { }

    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
    }
}