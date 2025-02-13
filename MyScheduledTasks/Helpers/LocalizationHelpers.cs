﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

/// <summary>
/// Class for localization and culture helper methods.
/// </summary>
internal static class LocalizationHelpers
{
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
