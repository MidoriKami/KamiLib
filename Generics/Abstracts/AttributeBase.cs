using System;

namespace KamiLib.AutomaticUserInterface;

public class AttributeBase : Attribute
{
    protected static string TryGetLocalizedString(string? key)
    {
        if (key is not null && KamiCommon.Localization is { } wrapper)
        {
            var resolvedString = wrapper.GetTranslatedString(key);
            
            return string.IsNullOrEmpty(resolvedString) ? $"[[{key}]]" : resolvedString;
        }

        return string.IsNullOrEmpty(key) ? "[[invalid key]]" : key;
    }
}