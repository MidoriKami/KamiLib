using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class LeftLabeledTabledDrawableAttribute : TabledDrawableAttribute
{

    protected LeftLabeledTabledDrawableAttribute(string? label, string category, int group) : base(label, category, group) { }
    
    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
    }
}