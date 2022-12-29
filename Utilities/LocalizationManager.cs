using System;
using System.IO;
using Dalamud;
using Dalamud.Logging;
using KamiLib.CommandSystem;

namespace KamiLib.Utilities;

public class LocalizationManager : IDisposable
{
    private readonly Localization? Localization;

    private static LocalizationManager? _instance;
    public static LocalizationManager Instance => _instance ??= new LocalizationManager();
    
    private LocalizationManager()
    {
        KamiCommon.CommandManager.AddCommand(new LocalizationCommand());
        
        var assemblyLocation = Service.PluginInterface.AssemblyLocation.DirectoryName!;
        var filePath = Path.Combine(assemblyLocation, @"translations");

        Localization = new Localization(filePath, $"{KamiCommon.PluginName}_");
        Localization.SetupWithLangCode(Service.PluginInterface.UiLanguage);

        Service.PluginInterface.LanguageChanged += OnLanguageChange;
    }

    public void ExportLocalization()
    {
        try
        {
            Localization?.ExportLocalizable();
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Error exporting localization files");
        }
    }

    public static void Cleanup()
    {
        _instance?.Dispose();
    }

    public void Dispose()
    {
        Service.PluginInterface.LanguageChanged -= OnLanguageChange;
    }

    private void OnLanguageChange(string languageCode)
    {
        try
        {
            PluginLog.Information($"Loading Localization for {languageCode}");
            Localization?.SetupWithLangCode(languageCode);
        }
        catch (Exception ex)
        {
            PluginLog.Error(ex, "Unable to Load Localization");
        }
    }
}