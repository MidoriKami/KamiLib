using Dalamud.Interface;
using Dalamud.Utility;
using ImGuiNET;
using KamiLib.Caching;
using KamiLib.Utilities;
using Lumina.Excel.GeneratedSheets;

namespace KamiLib.Extensions;

public static class TerritoryTypeExtensions
{
    public static void DrawLabel(this TerritoryType data)
    {
        var placeString = data.GetLabel();
        
        var startPosition = ImGui.GetCursorPos();
        ImGui.TextColored(Colors.Grey, data.RowId.ToString());
        ImGui.SameLine(startPosition.X + 50.0f * ImGuiHelpers.GlobalScale);
        ImGui.Text(placeString);
    }

    public static string GetLabel(this TerritoryType data)
    {
        var placeNameRow = data.PlaceName.Row;
        var placeName = PlaceNameCache.Instance.GetRow(placeNameRow);
        var placeString = placeName?.Name.ToDalamudString().TextValue ?? "Unknown PlaceName";

        return placeString;
    }
}