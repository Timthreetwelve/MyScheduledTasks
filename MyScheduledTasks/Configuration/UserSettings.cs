// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Configuration;

[INotifyPropertyChanged]
public partial class UserSettings : ConfigManager<UserSettings>
{
    #region Properties
    /// <summary>
    /// Add text to root folder.
    /// </summary>
    [ObservableProperty]
    private bool _annotateRoot = true;

    /// <summary>
    /// Height of the details pane.
    /// </summary>
    [ObservableProperty]
    private double _detailsHeight = 520;

    /// <summary>
    ///  Used to determine if Debug level messages are included in the application log.
    /// </summary>
    [ObservableProperty]
    private static double _dialogScale = 1;

    /// <summary>
    /// Don't display Microsoft folder.
    /// </summary>
    [ObservableProperty]
    private bool _hideMicrosoftFolder;

    /// <summary>
    /// Include debug level messages in the log file.
    /// </summary>
    [ObservableProperty]
    private bool _includeDebug = true;

    /// <summary>
    /// Keep window topmost.
    /// </summary>
    [ObservableProperty]
    private bool _keepOnTop;

    /// <summary>
    /// Enable language testing.
    /// </summary>
    [ObservableProperty]
    private bool _languageTesting;

    /// <summary>
    /// Accent color.
    /// </summary>
    [ObservableProperty]
    private AccentColor _primaryColor = AccentColor.Blue;

    /// <summary>
    /// Vertical spacing in the data grids.
    /// </summary>
    [ObservableProperty]
    private Spacing _rowSpacing = Spacing.Comfortable;

    /// <summary>
    /// Show the advanced menu.
    /// </summary>
    [ObservableProperty]
    private bool _showAdvancedMenu;

    /// <summary>
    /// Show the alert column in the grid.
    /// </summary>
    [ObservableProperty]
    private bool _showAlertCol = true;

    /// <summary>
    /// Show the details pane at the bottom.
    /// </summary>
    [ObservableProperty]
    private bool _showDetails = true;

    /// <summary>
    /// Show Exit in the navigation menu.
    /// </summary>
    [ObservableProperty]
    private bool _showExitInNav = true;

    /// <summary>
    /// Show the folder column.
    /// </summary>
    [ObservableProperty]
    private bool _showFolderCol = true;

    /// <summary>
    /// Show the last run column.
    /// </summary>
    [ObservableProperty]
    private bool _showLastRunCol = true;

    /// <summary>
    /// Show the next run column.
    /// </summary>
    [ObservableProperty]
    private bool _showNextRunCol;

    /// <summary>
    /// Show the note column.
    /// </summary>
    [ObservableProperty]
    private bool _showNoteCol = true;

    /// <summary>
    /// Show the Result column.
    /// </summary>
    [ObservableProperty]
    private bool _showResultCol = true;

    /// <summary>
    /// Show the Status column.
    /// </summary>
    [ObservableProperty]
    private bool _showStatusCol = true;

    /// <summary>
    /// Use sound.
    /// </summary>
    [ObservableProperty]
    private bool _sound = true;

    /// <summary>
    /// Option start with window centered on screen.
    /// </summary>
    [ObservableProperty]
    private bool _startCentered = true;

    /// <summary>
    /// Defined language to use in the UI.
    /// </summary>
    [ObservableProperty]
    private string _uILanguage = "en-US";

    /// <summary>
    /// Amount of UI zoom.
    /// </summary>
    [ObservableProperty]
    private MySize _uISize = MySize.Default;

    /// <summary>
    /// Theme type.
    /// </summary>
    [ObservableProperty]
    private ThemeType _uITheme = ThemeType.System;

    /// <summary>
    /// Use the operating system language (if one has been provided).
    /// </summary>
    [ObservableProperty]
    private bool _useOSLanguage;

    /// <summary>
    /// Height of the window.
    /// </summary>
    [ObservableProperty]
    private double _windowHeight = 800;

    /// <summary>
    /// Position of left side of the window.
    /// </summary>
    [ObservableProperty]
    private double _windowLeft = 100;

    /// <summary>
    /// Position of the top side of the window.
    /// </summary>
    [ObservableProperty]
    private double _windowTop = 100;

    /// <summary>
    /// Width of the window.
    /// </summary>
    [ObservableProperty]
    private double _windowWidth = 1400;
    #endregion Properties
}
