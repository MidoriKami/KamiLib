using System;
using System.ComponentModel;

namespace KamiLib.Classes;

public static class EnumExtensions {
    public static string GetDescription(this Enum value) {
        var type = value.GetType();
        if (Enum.GetName(type, value) is { } name) {
            if (type.GetField(name) is { } field) {
                if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attr) {
                    return attr.Description;
                }
            }
        }
        
        return value.ToString();
    }
}