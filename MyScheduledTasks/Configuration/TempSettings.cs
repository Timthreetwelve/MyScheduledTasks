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
    private static bool _langExpanderOpen;

    [ObservableProperty]
    private static bool _uIExpanderOpen;

    [ObservableProperty]
    private static bool _importAddToMyTasks;

    [ObservableProperty]
    private static string _importXMLFile;

    [ObservableProperty]
    private static string _importTaskName;

    [ObservableProperty]
    private static bool _importOverwrite;

    [ObservableProperty]
    private static bool _importRunOnlyLoggedOn = true;

    [ObservableProperty]
    private static bool _importResetCreationDate = true;
}
