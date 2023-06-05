using System;
using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntListDisplay : DrawableAttribute
{
    protected string EmptyListString = "NothingToTrack";
    
    public IntListDisplay(string? labelLocKey) : base(labelLocKey) { }
    
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null) => DrawTabled(obj, field);

    protected override void LeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.TextUnformatted(Label);
    }

    protected override void RightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var list = GetValue<List<int>>(obj, field);
        if (list.Count > 0)
        {
            foreach (var value in list)
            {
                ImGui.TextUnformatted(value.ToString());
            }
        }
        else
        {
            ImGui.TextUnformatted(TryGetLocalizedString(EmptyListString));
        }
    }
}