using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KamiLib.AutomaticUserInterface.Models;

internal class AttributeData
{
    public required MemberInfo Member { get; init; }
    public required DrawableAttribute Attribute { get; init; }
}

internal class CategoryData : IEquatable<CategoryData>
{
    public required int Group { get; init; }
    public required string CategoryLabel { get; init; }
    public bool Equals(CategoryData? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Group == other.Group && CategoryLabel == other.CategoryLabel;
    }
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CategoryData) obj);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Group, CategoryLabel);
    }
}

internal class OrderedAttributeData
{
    public required IDictionary<CategoryData, List<AttributeData>> OrderedData { get; init; }
}

internal class AttributeCache
{
    private readonly Dictionary<Type, OrderedAttributeData> cache = new();

    public OrderedAttributeData this[Type type]
    {
        get
        {
            if (cache.TryGetValue(type, out var item)) return item;

            cache.Add(type, GenerateForType(type));
            return cache[type];
        }
    }
    
    private OrderedAttributeData GenerateForType(Type type)
    {
        // Get all disabled entries
        var disabledMembers = new List<string>();

        foreach (var implementation in type.UnderlyingSystemType.GetInterfaces().Append(type))
        {
            foreach (var member in implementation.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                if(member is not {MemberType: MemberTypes.Field or MemberTypes.Property}) continue;
                if(member.IsDefined(typeof(Disabled))) disabledMembers.Add(member.Name);
            }
        }
        
        // Get all memberInfos
        var memberInfos = new Dictionary<CategoryData, List<AttributeData>>();

        foreach (var implementation in type.UnderlyingSystemType.GetInterfaces().Append(type))
        {
            if (implementation.GetCustomAttribute<CategoryAttribute>() is not { } categoryAttribute) continue;
            
            foreach (var member in implementation.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                if(member is not {MemberType: MemberTypes.Field or MemberTypes.Property}) continue;
                if(!member.IsDefined(typeof(DrawableAttribute), true)) continue;
                if (disabledMembers.Contains(member.Name)) continue;
                if (member.GetCustomAttribute<DrawableAttribute>() is not { } drawableAttribute) continue;

                var categoryData = new CategoryData
                {
                    Group = categoryAttribute.Index,
                    CategoryLabel = categoryAttribute.Category
                };

                var attributeData = new AttributeData
                {
                    Attribute = drawableAttribute,
                    Member = member,
                };

                if(!memberInfos.ContainsKey(categoryData)) memberInfos.Add(categoryData, new List<AttributeData>());
                
                memberInfos[categoryData].Add(attributeData);
            }
        }
        
        return new OrderedAttributeData
        {
            OrderedData = memberInfos
        };
    }
}