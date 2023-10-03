using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class RightLabeledTabledDrawableAttribute : TabledDrawableAttribute
{
    protected RightLabeledTabledDrawableAttribute(string? label) : base(label) { }

    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
    }
}