using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Interface;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class DrawableAttribute : AttributeBase
{
    private static readonly Dictionary<Type, IOrderedEnumerable<IGrouping<OrderData, AttributeData>>> AttributeCache = new();
    private record OrderData(int Group, string Label);
    private record AttributeData(FieldInfo Field, DrawableAttribute DrawableAttribute, DrawCategory DrawCategory);
    protected DrawableAttribute(string? labelLocKey) : base(labelLocKey) { }
    protected abstract void Draw(object obj, FieldInfo field, Action? saveAction = null);

    private static IOrderedEnumerable<IGrouping<OrderData, AttributeData>> GetSortedObjectAttributes(object configObject)
    {
        if (!AttributeCache.ContainsKey(configObject.GetType()))
        {
            var attributes = GetAttributes(configObject);
            
            var objectAttributes = attributes
                .GroupBy(attributeInfo => new OrderData(attributeInfo.DrawCategory.DrawIndex, attributeInfo.DrawCategory.Label))
                .OrderBy(orderData => orderData.Key.Group);

            AttributeCache.Add(configObject.GetType(), objectAttributes);
        }

        return AttributeCache[configObject.GetType()];
    }

    private static IEnumerable<AttributeData> GetAttributes(object obj)
    {
        var results = new List<AttributeData>();
        
        foreach (var field in obj.GetType().GetFields())
        {
            if (!field.IsDefined(typeof(DrawableAttribute), true)) continue;
            if (!field.IsDefined(typeof(DrawCategory), true)) continue;

            var configOptionAttribute = field.GetCustomAttribute<DrawableAttribute>();
            if (configOptionAttribute is null) continue;
            
            var drawCategoryAttribute = field.GetCustomAttribute<DrawCategory>();
            if (drawCategoryAttribute is null) continue;
            
            results.Add(new AttributeData(field, configOptionAttribute, drawCategoryAttribute));
        }

        return results;
    }

    protected void DrawTabled(object configObject, FieldInfo field, Action? saveAction = null)
    {
        if (!HasLabel)
        {
            LeftColumn(configObject, field, saveAction);
        }
        else
        {
            if (ImGui.BeginTable($"##ValueTable{Label}", 2, ImGuiTableFlags.SizingStretchSame))
            {
                ImGui.TableNextColumn();
                ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);

                LeftColumn(configObject, field, saveAction);

                ImGui.TableNextColumn();
                RightColumn(configObject, field, saveAction);

                ImGui.EndTable();
            }
        }
    }

    protected virtual void LeftColumn(object obj, FieldInfo field, Action? saveAction = null) { }
    protected virtual void RightColumn(object obj, FieldInfo field, Action? saveAction = null) { }
    
    public static void DrawAttributes(object obj, Action? saveAction = null)
    {
        var cachedAttributes = GetSortedObjectAttributes(obj);

        foreach (var categoryGroup in cachedAttributes)
        {
            ImGui.Text(categoryGroup.Key.Label);
            ImGui.Separator();
            ImGuiHelpers.ScaledIndent(15.0f);

            ImGui.PushID(categoryGroup.Key.Label);
            foreach (var attributeData in categoryGroup)
            {
                if (saveAction is not null)
                {
                    attributeData.DrawableAttribute.Draw(obj, attributeData.Field, saveAction);
                }
                else
                {
                    attributeData.DrawableAttribute.Draw(obj, attributeData.Field);
                }
            }
            ImGui.PopID();
            
            ImGuiHelpers.ScaledDummy(10.0f);
            ImGuiHelpers.ScaledIndent(-15.0f);
        }
    }
}