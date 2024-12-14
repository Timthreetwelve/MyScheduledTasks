// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.ViewModels;

internal sealed partial class NavigationViewModel : ObservableObject
{
    #region Constructor
    public NavigationViewModel()
    {
        if (CurrentViewModel == null)
        {
            NavigationItem main = FindNavPage(NavPage.Main);
            CurrentViewModel = Activator.CreateInstance((Type)main.ViewModelType!);
            PageTitle = main.PageTitle;
        }
    }
    #endregion Constructor

    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Properties
    [ObservableProperty]
    private object? _currentViewModel;

    [ObservableProperty]
    private string? _pageTitle;

    [ObservableProperty]
    private static NavigationItem? _navItem;
    #endregion Properties

    #region List of navigation items
    public static List<NavigationItem> NavigationViewModelTypes { get; set; } = new List<NavigationItem>
        (
            [
                new() {
                    Name=GetStringResource("NavItem_Main"),
                    NavPage = NavPage.Main,
                    ViewModelType= typeof(MainViewModel),
                    IconKind=PackIconKind.CheckboxOutline,
                    PageTitle=GetStringResource("NavTitle_Main")
                },
                new ()
                {
                    Name=GetStringResource("NavItem_AddTasks"),
                    NavPage = NavPage.AddTasks,
                    ViewModelType= typeof(AddTasksViewModel),
                    IconKind=PackIconKind.AddCircleOutline,
                    PageTitle = GetStringResource("NavTitle_AddTasks")
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
            ]
        );
    #endregion List of navigation items

    #region Navigation Methods
    internal static NavigationItem FindNavPage(NavPage page)
    {
        return NavigationViewModelTypes.Find(x => x.NavPage == page)!;
    }
    #endregion Navigation Methods

    #region Navigate Command
    [RelayCommand]
    internal void Navigate(object param)
    {
        if (param is NavigationItem item)
        {
            if (item.IsExit)
            {
                if (InputManager.Current.MostRecentInputDevice is KeyboardDevice)
                {
                    return;
                }
                Application.Current.Shutdown();
            }
            else if (item.ViewModelType is not null)
            {
                PageTitle = item.PageTitle;
                CurrentViewModel = null;
                CurrentViewModel = Activator.CreateInstance((Type)item.ViewModelType);
                NavItem = item;
            }
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

    #region View tasks file
    [RelayCommand]
    public static void OpenTasksFile()
    {
        TextFileViewer.ViewTextFile(TaskFileHelpers.TasksFile);
    }
    #endregion View tasks file

    #region View result codes file
    [RelayCommand]
    public static void ViewResultCodes()
    {
        TextFileViewer.ViewTextFile(Path.Combine(AppInfo.AppDirectory, "CommonCompletionCodes.txt"));
    }
    #endregion View result codes file

    #region Toggle details
    [RelayCommand]
    public static void ToggleDetails()
    {
        UserSettings.Setting!.ShowDetails = !UserSettings.Setting.ShowDetails;
        MainPage.Instance!.DetailsRow.Height = !UserSettings.Setting.ShowDetails
            ? new GridLength(1)
            : new GridLength(UserSettings.Setting.DetailsHeight);
    }
    #endregion Toggle details

    #region Remove column sort
    [RelayCommand]
    public static void RemoveSort()
    {
        MainPage.Instance!.ClearColumnSort();
    }
    #endregion Remove column sort

    #region Application Shutdown
    [RelayCommand]
    public static void Exit()
    {
        Application.Current.Shutdown();
    }
    #endregion Application Shutdown

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
            _log.Debug($"Launching Task Scheduler: {p.StartInfo.FileName} {p.StartInfo.Arguments}");
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error launching Task Scheduler: {ex.Message}");
        }
    }
    #endregion Open Task Scheduler

    #region Open the application folder
    [RelayCommand]
    public static void OpenAppFolder()
    {
        using Process process = new();
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.FileName = "Explorer.exe";
        process.StartInfo.Arguments = AppInfo.AppDirectory;
        _ = process.Start();
    }
    #endregion Open the application folder

    #region Check for new release
    [RelayCommand]
    private static async System.Threading.Tasks.Task CheckReleaseAsync()
    {
        await GitHubHelpers.CheckRelease();
    }
    #endregion Check for new release

    #region Edit Task Note
    [RelayCommand]
    public static async System.Threading.Tasks.Task EditNote()
    {
        DataGrid grid = MainPage.Instance!.DataGridTasks;
        ScheduledTask? row = grid.SelectedItem as ScheduledTask;
        await DialogHelpers.ShowEditNoteDialog(row!);
        System.Threading.Tasks.Task.Delay(100).Wait();
        grid.Items.Refresh();
    }
    #endregion Edit Task Note

    #region Add tasks
    [RelayCommand]
    public static void AddTasks()
    {
        _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.AddTasks);
    }
    #endregion Add tasks

    #region Remove tasks
    [RelayCommand]
    public static void RemoveTasks()
    {
        TaskHelpers.RemoveTasks(MainPage.Instance!.DataGridTasks);
    }
    #endregion Remove tasks

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
                            _mainWindow!,
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
        TaskFileHelpers.WriteTasksToFile();
    }
    #endregion Save task file

    #region UI Smaller and Larger
    [RelayCommand]
    public static void UILarger()
    {
        MainWindowHelpers.EverythingLarger();
    }

    [RelayCommand]
    public static void UISmaller()
    {
        MainWindowHelpers.EverythingSmaller();
    }
    #endregion UI Smaller and Larger

    #region Refresh
    [RelayCommand]
    public static void RefreshGrid()
    {
        int index = MainPage.Instance!.DataGridTasks.SelectedIndex;
        MainWindowHelpers.MainWindowWaitPointer();
        MainViewModel.LoadData();
        MainWindowHelpers.MainWindowNormalPointer();
        MainPage.Instance.DataGridTasks.SelectedIndex = index;
    }
    #endregion Refresh

    #region Export
    [RelayCommand]
    public static void ExportTasks()
    {
        TaskHelpers.ExportTask(MainPage.Instance!.DataGridTasks);
    }
    #endregion Export

    #region Import tasks
    [RelayCommand]
    public static void ShowImportTasks()
    {
        _ = DialogHelpers.ShowImportTaskDialog();
    }

    [RelayCommand]
    public static void ImportTask()
    {
        TaskHelpers.ImportTasks();
    }

    [RelayCommand]
    private static void ImportFilePicker()
    {
        ImportTaskDialog.FilePicker();
    }
    #endregion Import tasks

    #region Enable Tasks
    [RelayCommand]
    public static void EnableTasks()
    {
        TaskHelpers.EnableTask(MainPage.Instance!.DataGridTasks);
        RefreshGrid();
    }
    #endregion Enable Tasks

    #region Disable Tasks
    [RelayCommand]
    public static void DisableTasks()
    {
        TaskHelpers.DisableTask(MainPage.Instance!.DataGridTasks);
        RefreshGrid();
    }
    #endregion Disable Tasks

    #region Delete Tasks
    [RelayCommand]
    public static void ShowDeleteTasks()
    {
        _ = DialogHelpers.ShowDeleteTasksDialog(MainPage.Instance!.DataGridTasks);
    }

    [RelayCommand]
    private static void DeleteTasks()
    {
        TaskHelpers.DeleteTasks(MainPage.Instance!.DataGridTasks);
    }
    #endregion

    #region Run Tasks
    [RelayCommand]
    public static void RunTasks()
    {
        TaskHelpers.RunTask(MainPage.Instance!.DataGridTasks);
        System.Threading.Tasks.Task.Delay(100).Wait();
        RefreshGrid();
    }
    #endregion Run Tasks

    #region Open Choose Columns in Settings
    [RelayCommand]
    public static void ChooseColumns()
    {
        _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.Settings);
        TempSettings.Setting!.AppExpanderOpen = false;
        TempSettings.Setting!.ColumnsExpanderOpen = true;
        TempSettings.Setting!.LangExpanderOpen = false;
        TempSettings.Setting!.UIExpanderOpen = false;
    }
    #endregion Open Choose Columns in Settings

    #region Key down events
    /// <summary>
    /// Keyboard events
    /// </summary>
    [RelayCommand]
    private void KeyDown(KeyEventArgs e)
    {
        #region Keys without modifiers
        switch (e.Key)
        {
            case Key.F1:
                _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.About);
                e.Handled = true;
                break;
            case Key.F5:
                RefreshGrid();
                e.Handled = true;
                break;
            case Key.Escape:
                switch (CurrentViewModel)
                {
                    case MainViewModel:
                        MainPage.Instance!.DataGridTasks.SelectedIndex = -1;
                        Keyboard.ClearFocus();
                        e.Handled = true;
                        break;
                    case AddTasksViewModel:
                        Views.AddTasks.Instance!.AllTasksGrid.SelectedIndex = -1;
                        e.Handled = true;
                        break;
                    default:
                        e.Handled = true;
                        break;
                }
                break;
        }
        #endregion Keys without modifiers

        #region Keys with Ctrl
        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            switch (e.Key)
            {
                case Key.OemComma:
                    _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.Settings);
                    e.Handled = true;
                    break;
                case Key.D:
                    UserSettings.Setting!.ShowDetails = !UserSettings.Setting.ShowDetails;
                    MainPage.Instance!.DetailsRow.Height = !UserSettings.Setting.ShowDetails
                        ? new GridLength(1)
                        : new GridLength(UserSettings.Setting.DetailsHeight);
                    e.Handled = true;
                    break;
                case Key.L:
                    TaskHelpers.ListMyTasks(MainPage.Instance!.DataGridTasks);
                    e.Handled = true;
                    break;
                case Key.R:
                    RemoveSort();
                    e.Handled = true;
                    break;
                case Key.S:
                    SaveTasks();
                    e.Handled = true;
                    break;
                case Key.T:
                    _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.Main);
                    e.Handled = true;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    {
                        MainWindowHelpers.EverythingLarger();
                        ShowUIChangeMessage("size");
                        e.Handled = true;
                        break;
                    }
                case Key.Subtract:
                case Key.OemMinus:
                    {
                        MainWindowHelpers.EverythingSmaller();
                        ShowUIChangeMessage("size");
                        e.Handled = true;
                        break;
                    }
            }
        }
        #endregion Keys with Ctrl

        #region Keys with Ctrl and Shift
        if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
        {
            switch (e.Key)
            {
                case Key.C:
                    {
                        if (UserSettings.Setting!.PrimaryColor >= AccentColor.White)
                        {
                            UserSettings.Setting.PrimaryColor = AccentColor.Red;
                        }
                        else
                        {
                            UserSettings.Setting.PrimaryColor++;
                        }
                        ShowUIChangeMessage("color");
                        e.Handled = true;
                        break;
                    }
                case Key.F:
                    {
                        using Process p = new();
                        p.StartInfo.FileName = AppInfo.AppDirectory;
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.ErrorDialog = false;
                        _ = p.Start();
                        e.Handled = true;
                        break;
                    }
                case Key.K:
                    CompareLanguageDictionaries();
                    ViewLogFile();
                    e.Handled = true;
                    break;
                case Key.S:
                    TextFileViewer.ViewTextFile(ConfigHelpers.SettingsFileName!);
                    e.Handled = true;
                    break;
                case Key.T:
                    {
                        switch (UserSettings.Setting!.UITheme)
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
                        ShowUIChangeMessage("theme");
                        e.Handled = true;
                        break;
                    }
            }
        }
        #endregion Keys with Ctrl and Shift
    }
    #endregion Key down events

    #region Right mouse button
    /// <summary>
    /// Copy (nearly) any text in a TextBlock to the clipboard on right mouse button up.
    /// </summary>
    [RelayCommand]
    public static void RightMouseUp(MouseButtonEventArgs e)
    {
        if (e.OriginalSource is not TextBlock text)
        {
            return;
        }

        // Skip the DataGrid of tasks since it has a context menu
        DataGrid dg = MainWindowHelpers.FindParent<DataGrid>(text);
        if (dg?.Name == "DataGridTasks")
        {
            return;
        }

        if (ClipboardHelper.CopyTextToClipboard(text.Text))
        {
            SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_CopiedToClipboardItem"));
            _log.Debug($"{text.Text.Length} bytes copied to the clipboard");
        }
    }
    #endregion Right mouse button

    #region Show snack bar message for UI changes
    private static void ShowUIChangeMessage(string messageType)
    {
        CompositeFormat? composite = null;
        string messageVar = string.Empty;

        switch (messageType)
        {
            case "size":
                composite = MsgTextUISizeSet;
                messageVar = EnumHelpers.GetEnumDescription(UserSettings.Setting!.UISize);
                break;
            case "theme":
                composite = MsgTextUIThemeSet;
                messageVar = EnumHelpers.GetEnumDescription(UserSettings.Setting!.UITheme);
                break;
            case "color":
                composite = MsgTextUIColorSet;
                messageVar = EnumHelpers.GetEnumDescription(UserSettings.Setting!.PrimaryColor);
                break;
        }

        string message = string.Format(CultureInfo.InvariantCulture, composite!, messageVar);
        SnackbarMsg.ClearAndQueueMessage(message, 2000);
    }
    #endregion Show snack bar message for UI changes
}
