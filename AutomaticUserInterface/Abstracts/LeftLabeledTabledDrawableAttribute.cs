using System;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class LeftLabeledTabledDrawableAttribute : TabledDrawableAttribute
{

    protected LeftLabeledTabledDrawableAttribute(string? labelLocKey) : base(labelLocKey) { }
    
    protected override void DrawLeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
    }
}