using System;
using System.Collections.Generic;
using System.Reflection;
using Dalamud.Interface.Utility;
using ImGuiNET;
using KamiLib.AutomaticUserInterface.Models;

namespace KamiLib.AutomaticUserInterface;

public abstract class DrawableAttribute : FieldAttributeBase
{
    private static readonly AttributeCache AttributeCache = new();

    protected DrawableAttribute(string? label) : base(label)
    {
    }
    
    protected abstract void Draw(object obj, MemberInfo field, Action? saveAction = null);

    public static void DrawAttributes(object obj, Action? saveAction = null)
    {
        var attributeData = AttributeCache[obj.GetType()].OrderedData;

        foreach (var categoryGroup in attributeData)
        {
            DrawGroup(categoryGroup, obj, saveAction);
        }
    }

    private static void DrawGroup(KeyValuePair<CategoryData, List<AttributeData>> grouping, object sourceObject, Action? saveAction = null)
    {
        ImGui.Text(grouping.Key.CategoryLabel);
        ImGui.Separator();
        ImGuiHelpers.ScaledIndent(15.0f);

        ImGui.PushID(grouping.Key.CategoryLabel);
        foreach (var attributeData in grouping.Value)
        {
            attributeData.Attribute.Draw(sourceObject, attributeData.Member, saveAction);
        }
        ImGui.PopID();

        ImGuiHelpers.ScaledDummy(10.0f);
        ImGuiHelpers.ScaledIndent(-15.0f);
    }
}