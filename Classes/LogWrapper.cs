using Dalamud.IoC;
using Dalamud.Plugin.Services;

namespace KamiLib.Classes;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
internal class LogWrapper {
    [PluginService] public IPluginLog Log { get; set; }
}