using System;

namespace KamiLib.System;

public class LocalizationWrapper
{
    public required Func<string, string?> GetTranslatedString { get; init; }
}