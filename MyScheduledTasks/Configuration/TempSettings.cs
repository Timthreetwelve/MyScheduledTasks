// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Configuration;

/// <summary>
/// Class for non-persistent settings.
/// </summary>
[INotifyPropertyChanged]
internal partial class TempSettings : ConfigManager<TempSettings>
{
    [ObservableProperty]
    private static bool _appExpanderOpen;

    [ObservableProperty]
    private static bool _driveExpanderOpen;

    [ObservableProperty]
    private static bool _logicalExpanderOpen;

    [ObservableProperty]
    private static bool _physicalExpanderOpen;

    [ObservableProperty]
    private static int _driveSelectedTab;

    [ObservableProperty]
    private static bool _langExpanderOpen;

    [ObservableProperty]
    private static bool _uIExpanderOpen;

    [ObservableProperty]
    private static bool _runAccessPermitted;
}
