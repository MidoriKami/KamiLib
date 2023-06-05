using System;
using System.Globalization;
using System.Reflection;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public class FloatDisplay : DrawableAttribute
{

    public FloatDisplay(string? labelLocKey) : base(labelLocKey) { }
    protected override void Draw(object obj, FieldInfo field, Action? saveAction = null) => DrawTabled(obj, field);

    protected override void LeftColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        ImGui.Text(Label);
    }

    protected override void RightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var floatValue = GetValue<float>(obj, field);
        
        ImGui.TextUnformatted(floatValue.ToString(CultureInfo.CurrentCulture));
    }
}