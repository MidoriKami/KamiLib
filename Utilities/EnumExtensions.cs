using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Dalamud.Interface;
using Dalamud.Utility;
using KamiLib.AutomaticUserInterface;

namespace KamiLib.Utilities;

public static class EnumExtensions
{
    private static readonly Dictionary<Enum, EnumLabelAttribute?> LabelAttributeCache = new();
    private static readonly Dictionary<Enum, DisplayColorAttribute?> DisplayColorAttributeCache = new();

    public static string Label(this Enum enumValue) 
    {
        if (!LabelAttributeCache.ContainsKey(enumValue))
        {
            LabelAttributeCache.Add(enumValue, enumValue.GetAttribute<EnumLabelAttribute>());
        }

        var labelAttribute = LabelAttributeCache[enumValue];

        return labelAttribute is not null ? labelAttribute.Label : enumValue.ToString();
    }
    
    public static Vector4 Color(this Enum enumValue) 
    {
        if (!DisplayColorAttributeCache.ContainsKey(enumValue))
        {
            DisplayColorAttributeCache.Add(enumValue, enumValue.GetAttribute<DisplayColorAttribute>());
        }

        var colorAttribute = DisplayColorAttributeCache[enumValue];
        
        return colorAttribute?.Color.Vector() ?? KnownColor.White.Vector();
    }
}
