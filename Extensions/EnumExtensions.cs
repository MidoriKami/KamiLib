using System;
using System.ComponentModel;
using System.Resources;

namespace KamiLib.Extensions;

public static class EnumExtensions {
    public static string GetDescription(this Enum value, ResourceManager? resourceManager = null) {
        var type = value.GetType();
        if (Enum.GetName(type, value) is { } name) {
            if (type.GetField(name) is { } field) {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr) {
                    if (resourceManager?.GetString(attr.Description) is { } localized) {
                        return localized;
                    }

                    return attr.Description;
                }
            }
        }
        
        return value.ToString();
    }
}