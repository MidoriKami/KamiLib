using System;
using System.Reflection;

namespace KamiLib.AutomaticUserInterface;

public abstract class FieldAttributeBase : AttributeBase
{
    private readonly string? labelLocalizationKey;
    
    public string Label => TryGetLocalizedString(labelLocalizationKey);

    protected bool HasLabel => labelLocalizationKey is not null;
    
    protected FieldAttributeBase(string? label)
    {
        labelLocalizationKey = label;
    }

    protected T GetValue<T>(object obj, MemberInfo memberInfo)
    {
        try
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return (T) (memberInfo as FieldInfo)?.GetValue(obj)!;

                case MemberTypes.Property:
                    return (T) (memberInfo as PropertyInfo)?.GetValue(obj)!;

                default:
                    throw new Exception("Unsupported Member Type");
            }
        }
        catch (Exception e)
        {
            Service.Log.Error(e, "Reflection Error");
            throw;
        }
    }

    protected void SetValue<T>(object obj, MemberInfo memberInfo, T value)
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                (memberInfo as FieldInfo)?.SetValue(obj, value);
                break;

            case MemberTypes.Property:
                (memberInfo as PropertyInfo)?.SetValue(obj, value);
                break;

            default:
                throw new Exception("Unsupported Member Type");
        }
    }
}