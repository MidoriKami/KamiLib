using System;
using System.Drawing;
using System.Reflection;
using ImGuiNET;
using KamiLib.Utilities;

namespace KamiLib.AutomaticUserInterface;

public class BoolDisplay : LeftLabeledTabledDrawableAttribute
{
    protected string TrueString = "True";
    protected string FalseString = "False";

    protected KnownColor TrueColor = KnownColor.White;
    protected KnownColor FalseColor = KnownColor.White;

    public BoolDisplay(string? labelLocKey) : base(labelLocKey) { }
    
    protected override void DrawRightColumn(object obj, FieldInfo field, Action? saveAction = null)
    {
        var boolData = GetValue<bool>(obj, field);

        var color = boolData ? TrueColor.AsVector4() : FalseColor.AsVector4();
        var label = boolData ? TrueString : FalseString;
        
        ImGui.TextColored(color, label);
    }
}