using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class RightLabeledTabledDrawableAttribute : TabledDrawableAttribute
{
    protected RightLabeledTabledDrawableAttribute(string? labelLocKey) : base(labelLocKey) { }

    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
    }
}