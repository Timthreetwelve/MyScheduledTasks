// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Configuration;

[INotifyPropertyChanged]
public partial class UserSettings : ConfigManager<UserSettings>
{
    #region Properties
    [ObservableProperty]
    private bool _alertSound;

    [ObservableProperty]
    private bool _annotateRoot = true;

    [ObservableProperty]
    private double _detailsHeight = 520;

    [ObservableProperty]
    private static double _dialogScale = 1;

    [ObservableProperty]
    private bool _hideMicrosoftFolder;

    [ObservableProperty]
    private bool _includeDebug = true;

    [ObservableProperty]
    private bool _keepOnTop;

    [ObservableProperty]
    private bool _languageTesting;

    [ObservableProperty]
    private AccentColor _primaryColor = AccentColor.Blue;

    [ObservableProperty]
    private Spacing _rowSpacing = Spacing.Comfortable;

    [ObservableProperty]
    private bool _showAdvancedMenu;

    [ObservableProperty]
    private bool _showAlertCol = true;

    [ObservableProperty]
    private bool _showDetails = true;

    [ObservableProperty]
    private bool _showExitInNav = true;

    [ObservableProperty]
    private bool _showFolderCol = true;

    [ObservableProperty]
    private bool _showLastRunCol = true;

    [ObservableProperty]
    private bool _showNextRunCol;

    [ObservableProperty]
    private bool _showNoteCol = true;

    [ObservableProperty]
    private bool _showResultCol = true;

    [ObservableProperty]
    private bool _showStatusCol = true;

    [ObservableProperty]
    private bool _sound = true;

    [ObservableProperty]
    private bool _startCentered = true;

    [ObservableProperty]
    private string _uILanguage = "en-US";

    [ObservableProperty]
    private MySize _uISize = MySize.Default;

    [ObservableProperty]
    private ThemeType _uITheme = ThemeType.System;

    [ObservableProperty]
    private bool _useOSLanguage;

    [ObservableProperty]
    private double _windowHeight = 800;

    [ObservableProperty]
    private double _windowLeft = 100;

    [ObservableProperty]
    private double _windowTop = 100;

    [ObservableProperty]
    private double _windowWidth = 1400;
    #endregion Properties
}
