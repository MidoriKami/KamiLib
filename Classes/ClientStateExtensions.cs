using Dalamud.Plugin.Services;

namespace KamiLib.Classes;

public static class ClientStateExtensions {
    public static bool IsLoggedInNotPvP(this IClientState clientState, bool excludeDen = false) {
        if (clientState is { IsLoggedIn: false }) return false;
        if (clientState is { IsPvP: true, IsPvPExcludingDen: false } && excludeDen) return false;

        return true;
    }
}