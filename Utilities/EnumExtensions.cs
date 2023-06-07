using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Utility;
using KamiLib.AutomaticUserInterface;

namespace KamiLib.Utilities;

public static class EnumExtensions
{
    private static readonly Dictionary<Enum, EnumLabel?> LabelAttributeCache = new();
    private static readonly Dictionary<Enum, DisplayColor?> DisplayColorAttributeCache = new();

    public static string GetLabel(this Enum enumValue) 
    {
        if (!LabelAttributeCache.ContainsKey(enumValue))
        {
            LabelAttributeCache.Add(enumValue, enumValue.GetAttribute<EnumLabel>());
        }

        var labelAttribute = LabelAttributeCache[enumValue];

        return labelAttribute is not null ? labelAttribute.Description : enumValue.ToString();
    }
    
    public static Vector4 GetColor(this Enum enumValue) 
    {
        if (!DisplayColorAttributeCache.ContainsKey(enumValue))
        {
            DisplayColorAttributeCache.Add(enumValue, enumValue.GetAttribute<DisplayColor>());
        }

        var colorAttribute = DisplayColorAttributeCache[enumValue];
        
        return colorAttribute?.Color.AsVector4() ?? KnownColor.White.AsVector4();
    }

    public static Vector4 AsVector4(this KnownColor enumValue)
    {
        var enumColor = Color.FromKnownColor(enumValue);
        return new Vector4(enumColor.R / 255.0f, enumColor.G / 255.0f, enumColor.B / 255.0f, enumColor.A / 255.0f);
    }
}
