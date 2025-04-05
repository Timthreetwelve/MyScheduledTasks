// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

/// <summary>
/// Class for localization and culture helper methods.
/// </summary>
internal static class LocalizationHelpers
{
    #region Properties
    /// <summary>
    /// Uri of the resource dictionary
    /// </summary>
    private static string? LanguageFile { get; set; }

    /// <summary>
    /// Number of language strings in a resource dictionary
    /// </summary>
    public static int LanguageStrings { get; set; }
    #endregion Properties

    #region Return the current culture (language)
    /// <summary>
    /// Gets the current culture.
    /// </summary>
    /// <returns>Current culture name</returns>
    public static string GetCurrentCulture()
    {
        return CultureInfo.CurrentCulture.Name;
    }
    #endregion Return the current culture (language)

    #region Return the current UI culture
    /// <summary>
    /// Gets the current UI culture.
    /// </summary>
    /// <returns>Current UI culture name</returns>
    public static string GetCurrentUICulture()
    {
        return CultureInfo.CurrentUICulture.Name;
    }
    #endregion Return the current UI culture

    #region Apply language settings
    /// <summary>
    /// Apply language settings.
    /// </summary>
    /// <param name="LanguageDictionary">The resource dictionary corresponding to the selected language.</param>
    public static void ApplyLanguageSettings(ResourceDictionary LanguageDictionary)
    {
        LanguageStrings = LanguageDictionary.Count;
        LanguageFile = LanguageDictionary.Source.OriginalString;
        if (LanguageStrings == 0)
        {
            _log.Warn($"No strings loaded from {LanguageFile}");
        }
        _log.Debug($"Current culture: {GetCurrentCulture()}  UI: {GetCurrentUICulture()}");
        _log.Debug($"{LanguageStrings} strings loaded from {LanguageFile}");
    }
    #endregion Apply language settings

    #region Check if Use OS Language is set
    /// <summary>
    /// Check if the option to use the OS language is set and if the language is defined.
    /// </summary>
    /// <param name="language">The language code to check.</param>
    /// <returns>True if the language is defined and the language exists. Otherwise return false.</returns>
    public static bool CheckUseOsLanguage(string language)
    {
        if (UserSettings.Setting!.UseOSLanguage)
        {
            if (UILanguage.DefinedLanguages.Exists(x => x.LanguageCode == language))
            {
                return true;
            }
            _log.Warn($"Language \"{language}\" has not been defined in this application. Defaulting to en-US and setting \"Use OS Language\" to false.");
            UserSettings.Setting.UseOSLanguage = false;
            ConfigHelpers.SaveSettings();
        }
        return false;
    }
    #endregion Check if Use OS Language is set

    #region Save settings and restart (after language change)
    /// <summary>
    /// Saves settings and restarts the application. Invoked when language is changed.
    /// </summary>
    public static void SaveAndRestart()
    {
        ConfigHelpers.SaveSettings();
        using Process p = new();
        p.StartInfo.FileName = AppInfo.AppPath;
        p.StartInfo.UseShellExecute = true;
        _ = p.Start();
        _log.Debug("Restarting for language change.");
        Application.Current.Shutdown();
    }
    #endregion Save settings and restart (after language change)
}
