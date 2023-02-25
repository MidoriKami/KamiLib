using KamiLib.Localization;

namespace KamiLib.ZoneFilterList;

public enum ZoneFilterTypeId
{
    Whitelist,
    Blacklist
}

public class ZoneFilterType
{
    public static ZoneFilterType WhiteList { get; } = new(ZoneFilterTypeId.Whitelist, Strings.ZoneFilterList_ClearWhitelist,
                                                          Strings.ZoneFilterList_CurrentlyWhitelisted,
                                                          Strings.ZoneFilterList_EmptyWhitelist,
                                                          Strings.ZoneFilterList_SelectZonesToWhitelist);

    public static ZoneFilterType BlackList { get; } = new(ZoneFilterTypeId.Blacklist,
                                                          Strings.ZoneFilterList_ClearBlacklist,
                                                          Strings.ZoneFilterList_CurrentlyBlacklisted,
                                                          Strings.ZoneFilterList_EmptyBlacklist,
                                                          Strings.ZoneFilterList_SelectZonesToBlacklist);

    public ZoneFilterTypeId Id { get; }
    public string ClearButton { get; }
    public string CurrentLabel { get; }
    public string IsEmptyLabel { get; }
    public string SelectZonesLabel { get; }

    public static ZoneFilterType? FromId(ZoneFilterTypeId id) => id switch
    {
        ZoneFilterTypeId.Whitelist => WhiteList,
        ZoneFilterTypeId.Blacklist => BlackList,
        _ => null
    };

    private ZoneFilterType(
        ZoneFilterTypeId id, string clearButton, string currentLabel, string isEmptyLabel, string selectZonesLabel)
    {
        Id = id;
        ClearButton = clearButton;
        CurrentLabel = currentLabel;
        IsEmptyLabel = isEmptyLabel;
        SelectZonesLabel = selectZonesLabel;
    }
}
