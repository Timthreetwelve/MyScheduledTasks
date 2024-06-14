// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Configuration;

/// <summary>
/// Class to handle certain changes in user settings.
/// </summary>
public static class SettingChange
{
    #region User Setting change
    /// <summary>
    /// Handle changes in UserSettings
    /// </summary>
    public static void UserSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        object? newValue = GetPropertyValue(sender, e);
        _log.Debug($"Setting change: {e.PropertyName} New Value: {newValue}");

        switch (e.PropertyName)
        {
            case nameof(UserSettings.Setting.IncludeDebug):
                SetLogLevel((bool)newValue!);
                break;

            case nameof(UserSettings.Setting.UITheme):
                MainWindowHelpers.SetBaseTheme((ThemeType)newValue!);
                break;

            case nameof(UserSettings.Setting.PrimaryColor):
                MainWindowHelpers.SetPrimaryColor((AccentColor)newValue!);
                break;

            case nameof(UserSettings.Setting.UISize):
                MainWindowHelpers.UIScale(UserSettings.Setting!.UISize);
                break;

            case nameof(UserSettings.Setting.LanguageTesting):
            case nameof(UserSettings.Setting.UILanguage):
                LocalizationHelpers.SaveAndRestart();
                break;
        }
    }
    #endregion User Setting change

    #region Temp setting change
    /// <summary>
    /// Handle changes in TempSettings
    /// </summary>
    internal static void TempSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        object? newValue = GetPropertyValue(sender, e);
        // Write to trace level to avoid unnecessary message in log file
        _log.Trace($"Temp Setting change: {e.PropertyName} New Value: {newValue}");

        switch (e.PropertyName)
        {
            case nameof(TempSettings.Setting.ImportRunOnlyLoggedOn):
                if (!(bool)newValue!)
                {
                    TaskHelpers.ImportCaution();
                }
                break;
        }
    }
    #endregion Temp setting change

    #region Get property value
    /// <summary>
    /// Gets the value of the property
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns>An object containing the value of the property</returns>
    public static object? GetPropertyValue(object sender, PropertyChangedEventArgs e)
    {
        PropertyInfo? prop = sender.GetType().GetProperty(e.PropertyName!);
        return prop?.GetValue(sender, null);
    }
    #endregion Get property value
}
