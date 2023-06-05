using System;

namespace KamiLib.Localization;

public class LocalizationWrapper
{
    public required Func<string, string?> GetTranslatedString { get; init; }
}