using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace KamiLib.Extensions;

public static class EnumExtensions {

    public static Func<ResourceManager?>? GetResourceManagerFunc { get; set; }
    
    public static Func<CultureInfo?>? GetCultureInfoFunc { get; set; }
    
    public static string GetDescription(this Enum value) {
        var type = value.GetType();
        if (Enum.GetName(type, value) is { } name) {
            if (type.GetField(name) is { } field) {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr) {
                    if (GetResourceManagerFunc?.Invoke() is { } resourceManager && GetCultureInfoFunc?.Invoke() is { } cultureInfo) {
                        if (resourceManager.GetString(attr.Description, cultureInfo) is { } localizedString) {
                            return localizedString;
                        }
                    }

                    return attr.Description;
                }
            }
        }
        
        return value.ToString();
    }
}