using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class LeftLabeledTabledDrawableAttribute : TabledDrawableAttribute
{

    protected LeftLabeledTabledDrawableAttribute(string? label) : base(label) { }
    
    protected override void DrawLeftColumn(object obj, MemberInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
    }
}