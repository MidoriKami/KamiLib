using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Dalamud.Logging;

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
    public required IOrderedEnumerable<IGrouping<CategoryData, AttributeData>> OrderedData { get; init; }
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
    
    private static OrderedAttributeData GenerateForType(Type type) => new()
    {
        OrderedData = GetAttributeDataForType(type.UnderlyingSystemType.GetInterfaces().Append(type))
            .OrderBy(groups => groups.Key.Group)
            .ThenBy(groups => groups.Key.CategoryLabel)
    };

    private static IEnumerable<IGrouping<CategoryData, AttributeData>> GetAttributeDataForType(IEnumerable<Type> types)
    {
        var list = new List<IGrouping<CategoryData, AttributeData>>();
        
        foreach (var type in types)
        {
            list.AddRange(GetAttributeDataForType(type));
        }

        return list;
    }

    private static IEnumerable<IGrouping<CategoryData, AttributeData>> GetAttributeDataForType(Type type)
    {
        var members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
        var drawableEnabledMembers = GetDrawableEnabledMembers(members);

        var category = type.GetCustomAttribute<CategoryAttribute>()!;

        return drawableEnabledMembers
            .Select(member => new AttributeData
            {
                Attribute = member.GetCustomAttribute<DrawableAttribute>()!,
                Member = member
            })
            .GroupBy(data => new CategoryData
            {
                Group = category.Index,
                CategoryLabel = category.Category,
            });
    }

    private static IEnumerable<MemberInfo> GetDisabledMembers(IEnumerable<MemberInfo> members)
    {
        foreach (var member in members)
        {
            if(member is not { MemberType: MemberTypes.Field or MemberTypes.Property }) continue;

            if (member.IsDefined(typeof(Disabled), true)) yield return member;
        }
    }

    private static IEnumerable<MemberInfo> GetDrawableMembers(IEnumerable<MemberInfo> members)
    {
        foreach (var member in members)
        {
            if(member is not { MemberType: MemberTypes.Field or MemberTypes.Property }) continue;

            if (member.IsDefined(typeof(DrawableAttribute), true)) yield return member;
        }
    }

    private static IEnumerable<MemberInfo> GetDrawableEnabledMembers(IReadOnlyCollection<MemberInfo> members)
    {
        var disabledMembers = GetDisabledMembers(members);
        var drawableMembers = GetDrawableMembers(members);

        return drawableMembers.Where(drawableMember => !disabledMembers.Any(disabledMember => string.Equals(disabledMember.Name, drawableMember.Name)));
    }
}