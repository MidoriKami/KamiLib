using System.Diagnostics;
using Dalamud.Logging;

namespace KamiLib.Misc;

public static unsafe class Logging
{
    public static void PrintAddress(void* address, string label)
    {
        PrintAddress(new nint(address), label);
    }
    
    public static void PrintAddress(nint address, string label)
    {
        PluginLog.Debug($"{GetAddressString(address)} - {label}");
    }

    private static string GetAddressString(nint address)
    {
        if (Process.GetCurrentProcess().MainModule is { } mainModule)
        {
            var begin = mainModule.BaseAddress.ToInt64();
            var end = begin + mainModule.ModuleMemorySize;

            if (address > 0 && address >= begin && address <= end)
            {
                return $"ffxiv_dx11.exe+{(address - begin):X}";
            }
            else
            {
                return $"{address:X}";
            }
        }

        return "Unable To Get Module Info";
    }
}