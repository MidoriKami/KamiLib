using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dalamud.Interface;
using ImGuiNET;

namespace KamiLib.AutomaticUserInterface;

public abstract class DrawableAttribute : FieldAttributeBase
{
    private static readonly Dictionary<Type, IOrderedEnumerable<IGrouping<OrderData, AttributeData>>> AttributeCache = new();
    private record OrderData(int Group, string Label);
    private record AttributeData(FieldInfo Field, DrawableAttribute DrawableAttribute);
    protected DrawableAttribute(string? label, string category, int group) : base(label, category, group) { }
    protected abstract void Draw(object obj, FieldInfo field, Action? saveAction = null);

    private static IOrderedEnumerable<IGrouping<OrderData, AttributeData>> GetSortedObjectAttributes(object configObject)
    {
        if (!AttributeCache.ContainsKey(configObject.GetType()))
        {
            var attributes = GetAttributes(configObject);
            
            var objectAttributes = attributes
                .GroupBy(attributeInfo => new OrderData(attributeInfo.DrawableAttribute.GroupIndex, attributeInfo.DrawableAttribute.Category))
                .OrderBy(orderData => orderData.Key.Group);

            AttributeCache.Add(configObject.GetType(), objectAttributes);
        }

        return AttributeCache[configObject.GetType()];
    }

    private static IEnumerable<AttributeData> GetAttributes(object obj) => 
        from field in obj.GetType().GetFields() 
        where field.IsDefined(typeof(DrawableAttribute), true) 
        let drawableAttribute = field.GetCustomAttribute<DrawableAttribute>() 
        where drawableAttribute is not null 
        select new AttributeData(field, drawableAttribute);

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
                attributeData.DrawableAttribute.Draw(obj, attributeData.Field, saveAction);
            }
            ImGui.PopID();
            
            ImGuiHelpers.ScaledDummy(10.0f);
            ImGuiHelpers.ScaledIndent(-15.0f);
        }
    }
}