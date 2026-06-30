// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.ViewModels;

internal sealed partial class SettingsViewModel : ObservableObject
{
    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Properties
    public static List<FontFamily>? FontList { get; private set; }

    public IEnumerable<ThemeType> ThemeTypes { get; private set; }

    public IEnumerable<ThemeType> SystemThemeTypes { get; private set; }
    #endregion Properties

    #region Constructor
    public SettingsViewModel()
    {
        FontList ??= [.. Fonts.SystemFontFamilies.OrderBy(x => x.Source)];

        // List of all themes available in the app. Will appear in settings in the order they are listed here.
        // System theme is last because it is not a real theme, but rather a setting to use the system theme.
        ThemeTypes = [
            ThemeType.Light,
            ThemeType.LightGray,
            ThemeType.Dark,
            ThemeType.Darker,
            ThemeType.DarkBlue,
            ThemeType.System,
        ];

        // Used when ThemeType.System is selected. Will display all themes except System theme
        SystemThemeTypes = ThemeTypes.Where(t => t != ThemeType.System);
    }

    #endregion Constructor

    #region Open App folder
    [RelayCommand]
    private static void OpenAppFolder()
    {
        string filePath = string.Empty;
        try
        {
            filePath = Path.Combine(AppInfo.AppDirectory, "Strings.test.xaml");
            if (File.Exists(filePath))
            {
                _ = Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }
            else
            {
                using Process p = new();
                p.StartInfo.FileName = AppInfo.AppDirectory;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error trying to open {PathHelpers.AnonymizePath(filePath)}: {ex.Message}");
            _ = new MDCustMsgBox(GetStringResource("MsgText_Error_FileExplorer"),
                     "My Scheduled Tasks ERROR",
                     ButtonType.Ok,
                     false,
                     true,
                     _mainWindow,
                     true).ShowDialog();
        }
    }
    #endregion Open App folder

    #region Open settings
    [RelayCommand]
    private static void OpenSettings()
    {
        ConfigHelpers.SaveSettings();
        TextFileViewer.ViewTextFile(ConfigHelpers.SettingsFileName!);
    }
    #endregion Open settings

    #region Export settings
    [RelayCommand]
    private static void ExportSettings()
    {
        ConfigHelpers.ExportSettings();
    }
    #endregion Export settings

    #region Import settings
    [RelayCommand]
    private static void ImportSettings()
    {
        ConfigHelpers.ImportSettings();
    }
    #endregion Import settings

    #region List (dump) settings to log file
    [RelayCommand]
    private static void DumpSettings()
    {
        ConfigHelpers.DumpSettings();
        NavigationViewModel.ViewLogFile();
    }
    #endregion List (dump) settings to log file

    #region Compare languages
    [RelayCommand]
    private static void CompareLanguageKeys()
    {
        CompareLanguageDictionaries();
        TextFileViewer.ViewTextFile(GetLogfileName());
    }
    #endregion Compare languages
}
