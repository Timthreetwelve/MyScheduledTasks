// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.ViewModels;
internal partial class NavigationViewModel : ObservableObject
{
    #region Constructor
    public NavigationViewModel()
    {
        if (CurrentViewModel == null)
        {
            NavigationItem main = FindNavPage(NavPage.Main);
            CurrentViewModel = Activator.CreateInstance((Type)main.ViewModelType);
            PageTitle = main.PageTitle;
        }
    }
    #endregion Constructor

    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Properties
    [ObservableProperty]
    private object _currentViewModel;

    [ObservableProperty]
    private string _pageTitle;

    [ObservableProperty]
    private static NavigationItem _navItem;
    #endregion Properties

    #region List of navigation items
    public static List<NavigationItem> NavigationViewModelTypes { get; set; } = new List<NavigationItem>
        (new List<NavigationItem>
            {
                new() {
                    Name=GetStringResource("NavItem_Main"),
                    NavPage = NavPage.Main,
                    ViewModelType= typeof(MainViewModel),
                    IconKind=PackIconKind.Microsoft,
                    PageTitle=GetStringResource("NavTitle_Main")
                },
                new ()
                {
                    Name="Add Tasks",
                    NavPage = NavPage.Main,
                    ViewModelType= typeof(AddTasksViewModel),
                    IconKind=PackIconKind.Add,
                    PageTitle = "Add Tasks"
                },
                new() {
                    Name = GetStringResource("NavItem_Settings"),
                    NavPage=NavPage.Settings,
                    ViewModelType= typeof(SettingsViewModel),
                    IconKind=PackIconKind.SettingsOutline,
                    PageTitle = GetStringResource("NavTitle_Settings")
                },
                new() {
                    Name = GetStringResource("NavItem_About"),
                    NavPage=NavPage.About,
                    ViewModelType= typeof(AboutViewModel),
                    IconKind=PackIconKind.AboutCircleOutline,
                    PageTitle = GetStringResource("NavTitle_About")
                },
                new() {
                    Name = GetStringResource("NavItem_Exit"),
                    IconKind=PackIconKind.ExitToApp,
                    IsExit=true
                }
            }
        );
    #endregion List of navigation items

    #region Navigation Methods
    private static NavigationItem FindNavPage(NavPage page)
    {
        return NavigationViewModelTypes.Find(x => x.NavPage == page);
    }
    #endregion Navigation Methods

    #region Navigate Command
    [RelayCommand]
    internal void Navigate(SelectionChangedEventArgs e)
    {
        ListBox listBox = e.Source as ListBox;
        NavigationItem oldItem = e.RemovedItems[0] as NavigationItem;
        NavigationItem selectedItem = e.AddedItems[0] as NavigationItem;

        if (selectedItem.IsExit)
        {
            if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
            {
                return;
            }
            Application.Current.Shutdown();
        }
        else if (selectedItem.IsRestart)
        {
            QueryRestartAsAdmin();
        }
        else if (selectedItem.IsLaunch)
        {
            listBox.SelectedItem = oldItem;
            try
            {
                using Process p = new();
                p.StartInfo.FileName = selectedItem.LaunchFile;
                p.StartInfo.ErrorDialog = false;
                p.StartInfo.UseShellExecute = selectedItem.LaunchShellExecute;
                if (selectedItem.LaunchArguments != null)
                {
                    p.StartInfo.Arguments = selectedItem.LaunchArguments;
                }
                _ = p.Start();
                Debug.WriteLine($"Starting {p.ProcessName}");
                Debug.WriteLine(NavItem.Name);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error launching {selectedItem.PageTitle}: {ex.Message}");
            }
        }
        else if (selectedItem.ViewModelType is not null)
        {
            PageTitle = selectedItem.PageTitle;
            CurrentViewModel = null;
            CurrentViewModel = Activator.CreateInstance((Type)selectedItem.ViewModelType);
            NavItem = selectedItem;
        }
    }
    #endregion Navigate Command

    #region View log file
    [RelayCommand]
    public static void ViewLogFile()
    {
        TextFileViewer.ViewTextFile(NLogHelpers.GetLogfileName());
    }
    #endregion View log file

    #region View readme file
    [RelayCommand]
    public static void ViewReadMeFile()
    {
        TextFileViewer.ViewTextFile(Path.Combine(AppInfo.AppDirectory, "readme.txt"));
    }
    #endregion View readme file

    #region Toggle details
    [RelayCommand]
    public static void ToggleDetails()
    {
        UserSettings.Setting.ShowDetails = !UserSettings.Setting.ShowDetails;
        MainPage.Instance.detailsRow.Height = !UserSettings.Setting.ShowDetails
            ? new GridLength(1)
            : new GridLength(UserSettings.Setting.DetailsHeight);
    }
    #endregion Toggle details

    #region Remove column sort
    [RelayCommand]
    public static void RemoveSort()
    {
        MainPage.Instance.ClearColumnSort();
    }
    #endregion Remove column sort

    #region Application Shutdown
    [RelayCommand]
    public static void Exit()
    {
        Application.Current.Shutdown();
    }
    #endregion Application Shutdown

    #region Add tasks
    [RelayCommand]
    public static void AddTasks()
    {
        DialogHelpers.ShowAddTasksDialog();
    }
    #endregion Add tasks

    #region Remove tasks
    [RelayCommand]
    public static void RemoveTasks()
    {
        Debug.WriteLine("Remove");
    }
    #endregion Remove tasks

    #region Open Task Scheduler
    [RelayCommand]
    public static void OpenTScheduler()
    {
        try
        {
            using Process p = new();
            p.StartInfo.FileName = "mmc.exe";
            p.StartInfo.ErrorDialog = false;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.Arguments = @"c:\windows\system32\taskschd.msc";
            _ = p.Start();
            Debug.WriteLine($"Starting {p.ProcessName}");
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error launching Task Scheduler: {ex.Message}");
        }
    }
    #endregion Open Task Scheduler


    #region Edit Task Note
    [RelayCommand]
    public static void EditNote()
    {
        DataGrid grid = MainPage.Instance.DataGridTasks;
        ScheduledTask row = grid.SelectedItem as ScheduledTask;
        DialogHelpers.ShowEditNoteDialog(row);
    }
    #endregion Edit Task Note
    #region Restart as Administrator
    /// <summary>
    /// Confirm restart
    /// </summary>
    [RelayCommand]
    public static void QueryRestartAsAdmin()
    {
        MDCustMsgBox mbox = new("Restart as Administrator?",
                            "Restart with Elevated Permissions?",
                            ButtonType.YesNo,
                            false,
                            true,
                            _mainWindow,
                            false);
        _ = mbox.ShowDialog();

        if (MDCustMsgBox.CustResult == CustResultType.Yes)
        {
            _log.Info($"{AppInfo.AppName} is restarting as Administrator");
            RestartAsAdmin();
        }
    }

    /// <summary>
    /// Restart as Administrator
    /// </summary>
    public static void RestartAsAdmin()
    {
        using Process p = new();
        p.StartInfo.FileName = AppInfo.AppPath;
        p.StartInfo.UseShellExecute = true;
        p.StartInfo.Verb = "runas";
        p.Start();
        Application.Current.Shutdown();
    }
    #endregion Restart as Administrator

    #region Save task file
    [RelayCommand]
    public static void SaveTasks()
    {
        TaskFileHelpers.WriteTasks2Json();
    }
    #endregion Save task file

    #region UI Smaller and Larger
    [RelayCommand]
    public static void UILarger()
    {
        MainWindowUIHelpers.EverythingLarger();
    }

    [RelayCommand]
    public static void UISmaller()
    {
        MainWindowUIHelpers.EverythingSmaller();
    }
    #endregion UI Smaller and Larger

    #region Refresh
    [RelayCommand]
    public static void RefreshGrid()
    {
        Debug.WriteLine("Refresh");
    }
    #endregion Refresh

    #region Export
    [RelayCommand]
    public static void ExportTasks()
    {
        Debug.WriteLine("Export");
    }
    #endregion Export

    [RelayCommand]
    public static void ShowImportTasks()
    {
        DialogHelpers.ShowImportTaskDialog();
    }

    [RelayCommand]
    public static void ImportTask()
    {
        TaskHelpers.ImportTasks();
    }

    #region Enable Tasks
    [RelayCommand]
    public static void EnableTasks()
    {
        Debug.WriteLine("Enable");
    }
    #endregion Enable

    #region Disable
    [RelayCommand]
    public static void DisableTasks()
    {
        Debug.WriteLine("Disable");
    }
    #endregion Disable

    #region Run
    [RelayCommand]
    public static void RunTasks()
    {
        Debug.WriteLine("Run");
    }
    #endregion Run

    #region Key down events
    /// <summary>
    /// Keyboard events
    /// </summary>
    [RelayCommand]
    public static void KeyDown(KeyEventArgs e)
    {
        #region Keys without modifiers
        switch (e.Key)
        {
            case Key.F1:
                {
                    _mainWindow.NavigationListBox.SelectedValue = FindNavPage(NavPage.About);
                    break;
                }
            case Key.Home:
                {
                    _mainWindow.NavigationListBox.SelectedValue = FindNavPage(NavPage.Main);
                    break;
                }
            case Key.Escape:
                {
                    MainPage.Instance.DataGridTasks.SelectedIndex = -1;
                    break;
                }
        }
        #endregion Keys without modifiers

        #region Keys with Ctrl
        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            switch (e.Key)
            {
                case Key.OemComma:
                    {
                        _mainWindow.NavigationListBox.SelectedValue = FindNavPage(NavPage.Settings);
                        break;
                    }
                case Key.C:
                    {
                        //CopyToClipboard();
                        break;
                    }
                case Key.D:
                    {
                        UserSettings.Setting.ShowDetails = !UserSettings.Setting.ShowDetails;
                        MainPage.Instance.detailsRow.Height = !UserSettings.Setting.ShowDetails
                            ? new GridLength(1)
                            : new GridLength(UserSettings.Setting.DetailsHeight);
                        break;
                    }
                case Key.N:
                    {
                        DialogHelpers.ShowAddTasksDialog();
                        break;
                    }
                case Key.Add:
                    {
                        MainWindowUIHelpers.EverythingLarger();
                        string size = EnumDescConverter.GetEnumDescription(UserSettings.Setting.UISize);
                        string message = string.Format(GetStringResource("MsgText_UISizeSet"), size);
                        SnackbarMsg.ClearAndQueueMessage(message, 2000);
                        break;
                    }
                case Key.Subtract:
                    {
                        MainWindowUIHelpers.EverythingSmaller();
                        string size = EnumDescConverter.GetEnumDescription(UserSettings.Setting.UISize);
                        string message = string.Format(GetStringResource("MsgText_UISizeSet"), size);
                        SnackbarMsg.ClearAndQueueMessage(message, 2000);
                        break;
                    }
            }
        }
        #endregion Keys with Ctrl

        #region Keys with Ctrl and Shift
        if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
        {
            if (e.Key == Key.T)
            {
                switch (UserSettings.Setting.UITheme)
                {
                    case ThemeType.Light:
                        UserSettings.Setting.UITheme = ThemeType.Dark;
                        break;
                    case ThemeType.Dark:
                        UserSettings.Setting.UITheme = ThemeType.Darker;
                        break;
                    case ThemeType.Darker:
                        UserSettings.Setting.UITheme = ThemeType.System;
                        break;
                    case ThemeType.System:
                        UserSettings.Setting.UITheme = ThemeType.Light;
                        break;
                }
                string theme = EnumDescConverter.GetEnumDescription(UserSettings.Setting.UITheme);
                string message = string.Format(GetStringResource("MsgText_UIThemeSet"), theme);
                SnackbarMsg.ClearAndQueueMessage(message, 2000);
            }
            if (e.Key == Key.C)
            {
                if (UserSettings.Setting.PrimaryColor >= AccentColor.White)
                {
                    UserSettings.Setting.PrimaryColor = AccentColor.Red;
                }
                else
                {
                    UserSettings.Setting.PrimaryColor++;
                }
                string color = EnumDescConverter.GetEnumDescription(UserSettings.Setting.PrimaryColor);
                string message = string.Format(GetStringResource("MsgText_UIColorSet"), color);
                SnackbarMsg.ClearAndQueueMessage(message, 2000);
            }
            if (e.Key == Key.F)
            {
                using Process p = new();
                p.StartInfo.FileName = AppInfo.AppDirectory;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
            }
            if (e.Key == Key.S)
            {
                TextFileViewer.ViewTextFile(ConfigHelpers.SettingsFileName);
            }
        }
        #endregion Keys with Ctrl and Shift
    }
    #endregion Key down events

}
