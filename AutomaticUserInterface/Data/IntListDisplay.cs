using System;
using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntListDisplay : LeftLabeledTabledDrawableAttribute
{
    protected string EmptyListString = "NothingToTrack";
    
    public IntListDisplay(string? label, string category, int group) : base(label, category, group) { }
    
    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
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