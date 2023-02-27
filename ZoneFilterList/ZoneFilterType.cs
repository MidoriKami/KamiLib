using KamiLib.Localization;

namespace KamiLib.ZoneFilterList;

public enum ZoneFilterTypeId
{
    Whitelist,
    Blacklist
}

public class ZoneFilterType
{
    public static ZoneFilterType WhiteList { get; } = new()
    {
        Id = ZoneFilterTypeId.Whitelist,
        ClearButton = Strings.ZoneFilterList_ClearWhitelist,
        CurrentLabel = Strings.ZoneFilterList_CurrentlyWhitelisted,
        IsEmptyLabel = Strings.ZoneFilterList_EmptyWhitelist,
        SelectZonesLabel = Strings.ZoneFilterList_SelectZonesToWhitelist
    };
    
    public static ZoneFilterType BlackList { get; } = new()
    {
        Id = ZoneFilterTypeId.Blacklist,
        ClearButton = Strings.ZoneFilterList_ClearBlacklist,
        CurrentLabel = Strings.ZoneFilterList_CurrentlyBlacklisted,
        IsEmptyLabel = Strings.ZoneFilterList_EmptyBlacklist,
        SelectZonesLabel = Strings.ZoneFilterList_SelectZonesToBlacklist
    };
    
    public ZoneFilterTypeId Id { get; init; }
    public string ClearButton { get; init; }
    public string CurrentLabel { get; init; }
    public string IsEmptyLabel { get; init; }
    public string SelectZonesLabel { get; init; }

    public static ZoneFilterType? FromId(ZoneFilterTypeId id) => id switch
    {
        ZoneFilterTypeId.Whitelist => WhiteList,
        ZoneFilterTypeId.Blacklist => BlackList,
        _ => null
    };
}
