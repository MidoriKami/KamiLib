using System;
using System.Collections.Generic;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class IntListDisplayAttribute : LeftLabeledTabledDrawableAttribute
{
    protected string EmptyListString = "NothingToTrack";

    public IntListDisplayAttribute(string? label) : base(label)
    {
    }

    protected override void DrawRightColumn(object obj, MemberInfo field, Action? saveAction = null)
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