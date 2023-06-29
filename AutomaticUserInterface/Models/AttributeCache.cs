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
    public required IOrderedEnumerable<IGrouping<CategoryData, AttributeData>> OrderedData { get; set; }
}

internal class AttributeCache
{
    private readonly Dictionary<Type, OrderedAttributeData> cache = new();
    private readonly Dictionary<Type, List<string>> disabledMembers = new();

    public OrderedAttributeData this[Type type]
    {
        get
        {
            if (cache.TryGetValue(type, out var item)) return item;

            foreach (var group in GenerateForType(type))
            {
                foreach (var attribute in group)
                {
                    if (disabledMembers.TryGetValue(type, out var disabled))
                    {
                        if (disabled.Contains(attribute.Member.Name))
                        {
                            
                        }
                    }
                }
            }
            
            var generated = GenerateForType(type).OrderedData.ToList()
                .RemoveAll(group => group.);
            
            cache.Add(type, generated);
            return cache[type];
        }
    }

    private IOrderedEnumerable<IGrouping<CategoryData, AttributeData>> RemoveDisabled(IOrderedEnumerable<IGrouping<CategoryData, AttributeData>> generated, Type type)
    {
        foreach (var group in GenerateForType(type))
        {
            foreach (var attribute in group)
            {
                if (disabledMembers.TryGetValue(type, out var disabled))
                {
                    if (disabled.Contains(attribute.Member.Name))
                    {
                        yield return group;
                    }
                }
            }
        }
    }

    private IOrderedEnumerable<IGrouping<CategoryData, AttributeData>> GenerateForType(Type type)
        => GetAttributeDataForType(type.UnderlyingSystemType.GetInterfaces().Append(type))
            .OrderBy(groups => groups.Key.Group)
            .ThenBy(groups => groups.Key.CategoryLabel);

    private IEnumerable<IGrouping<CategoryData, AttributeData>> GetAttributeDataForType(IEnumerable<Type> types)
    {
        var list = new List<IGrouping<CategoryData, AttributeData>>();
        
        foreach (var type in types)
        {
            list.AddRange(GetAttributeDataForType(type));
        }

        return list;
    }

    private IEnumerable<IGrouping<CategoryData, AttributeData>> GetAttributeDataForType(Type type)
    {
        var members = type.GetMembers(BindingFlags.Public | BindingFlags.Instance);
        var drawableEnabledMembers = GetDrawableEnabledMembers(members, type);

        var category = type.GetCustomAttribute<CategoryAttribute>()!;

        return drawableEnabledMembers
            .Select(member => new AttributeData
            {
                Attribute = member.GetCustomAttribute<DrawableAttribute>()!,
                Member = member
            })
            .GroupBy(_ => new CategoryData
            {
                Group = category.Index,
                CategoryLabel = category.Category,
            });
    }

    private IEnumerable<MemberInfo> GetDisabledMembers(IEnumerable<MemberInfo> members)
    {
        foreach (var member in members)
        {
            if(member is not { MemberType: MemberTypes.Field or MemberTypes.Property }) continue;

            if (member.IsDefined(typeof(Disabled))) yield return member;
        }
    }

    private IEnumerable<MemberInfo> GetDrawableMembers(IEnumerable<MemberInfo> members)
    {
        foreach (var member in members)
        {
            if(member is not { MemberType: MemberTypes.Field or MemberTypes.Property }) continue;

            if (member.IsDefined(typeof(DrawableAttribute), true)) yield return member;
        }
    }

    private IEnumerable<MemberInfo> GetDrawableEnabledMembers(IReadOnlyCollection<MemberInfo> members, Type sourceType)
    {
        var disabledMemberList = GetDisabledMembers(members).ToList();
        var drawableMembers = GetDrawableMembers(members);
        
        if(!disabledMembers.ContainsKey(sourceType)) disabledMembers.Add(sourceType, new List<string>());
        
        disabledMembers[sourceType].AddRange(disabledMemberList.Select(member => member.Name));

        return drawableMembers.Where(drawableMember => !disabledMemberList.Any(disabledMember => string.Equals(disabledMember.Name, drawableMember.Name)));
    }
}