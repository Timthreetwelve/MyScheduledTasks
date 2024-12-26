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
    public static List<NavigationItem> NavigationViewModelTypes { get; set; } =
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
            ];

    #endregion List of navigation items

    #region Navigation Methods
    internal static NavigationItem FindNavPage(NavPage page)
    {
        return NavigationViewModelTypes.Find(x => x.NavPage == page)!;
    }
    #endregion Navigation Methods

    #region Navigate Command
    [RelayCommand]
    private void Navigate(object param)
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
        TextFileViewer.ViewTextFile(GetLogfileName());
    }
    #endregion View log file

    #region View readme file
    [RelayCommand]
    private static void ViewReadMeFile()
    {
        TextFileViewer.ViewTextFile(Path.Combine(AppInfo.AppDirectory, "readme.txt"));
    }
    #endregion View readme file

    #region View tasks file
    [RelayCommand]
    private static void OpenTasksFile()
    {
        TextFileViewer.ViewTextFile(TaskFileHelpers.TasksFile);
    }
    #endregion View tasks file

    #region View result codes file
    [RelayCommand]
    private static void ViewResultCodes()
    {
        TextFileViewer.ViewTextFile(Path.Combine(AppInfo.AppDirectory, "CommonCompletionCodes.txt"));
    }
    #endregion View result codes file

    #region Toggle details
    [RelayCommand]
    private static void ToggleDetails()
    {
        UserSettings.Setting!.ShowDetails = !UserSettings.Setting.ShowDetails;
        MainPage.Instance!.DetailsRow.Height = !UserSettings.Setting.ShowDetails
            ? new GridLength(1)
            : new GridLength(UserSettings.Setting.DetailsHeight);
    }
    #endregion Toggle details

    #region Remove column sort
    [RelayCommand]
    private static void RemoveSort()
    {
        MainPage.Instance!.ClearColumnSort();
    }
    #endregion Remove column sort

    #region Application Shutdown
    [RelayCommand]
    private static void Exit()
    {
        Application.Current.Shutdown();
    }
    #endregion Application Shutdown

    #region Open Task Scheduler
    [RelayCommand]
    private static void OpenTScheduler()
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
    private static void OpenAppFolder()
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
    private static async System.Threading.Tasks.Task EditNote()
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
    private static void AddTasks()
    {
        _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.AddTasks);
    }
    #endregion Add tasks

    #region Remove tasks
    [RelayCommand]
    private static void RemoveTasks()
    {
        TaskHelpers.RemoveTasks(MainPage.Instance!.DataGridTasks);
    }
    #endregion Remove tasks

    #region Restart as Administrator
    /// <summary>
    /// Confirm restart
    /// </summary>
    [RelayCommand]
    private static void QueryRestartAsAdmin()
    {
        MDCustMsgBox mbox = new("Restart as Administrator?",
                            "Restart with Elevated Permissions?",
                            ButtonType.YesNo,
                            false,
                            true,
                            _mainWindow);
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
    private static void SaveTasks()
    {
        TaskFileHelpers.WriteTasksToFile();
    }
    #endregion Save task file

    #region UI Smaller and Larger
    [RelayCommand]
    private static void UILarger()
    {
        MainWindowHelpers.EverythingLarger();
    }

    [RelayCommand]
    private static void UISmaller()
    {
        MainWindowHelpers.EverythingSmaller();
    }
    #endregion UI Smaller and Larger

    #region Refresh
    [RelayCommand]
    private static void RefreshGrid()
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
    private static void ExportTasks()
    {
        TaskHelpers.ExportTask(MainPage.Instance!.DataGridTasks);
    }
    #endregion Export

    #region Import tasks
    [RelayCommand]
    private static void ShowImportTasks()
    {
        _ = DialogHelpers.ShowImportTaskDialog();
    }

    [RelayCommand]
    private static void ImportTask()
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
    private static void EnableTasks()
    {
        TaskHelpers.EnableTask(MainPage.Instance!.DataGridTasks);
        RefreshGrid();
    }
    #endregion Enable Tasks

    #region Disable Tasks
    [RelayCommand]
    private static void DisableTasks()
    {
        TaskHelpers.DisableTask(MainPage.Instance!.DataGridTasks);
        RefreshGrid();
    }
    #endregion Disable Tasks

    #region Delete Tasks
    [RelayCommand]
    private static void ShowDeleteTasks()
    {
        MainViewModel.TasksToDelete.Clear();
        for (int i = 0; i < MainPage.Instance!.DataGridTasks.SelectedItems.Count; i++)
        {
            ScheduledTask? task = MainPage.Instance.DataGridTasks.SelectedItems[i] as ScheduledTask;
            MainViewModel.TasksToDelete.Add(task!);
        }
        _ = DialogHelpers.ShowDeleteTasksDialog();
    }

    [RelayCommand]
    private static void DeleteTasks()
    {
        TaskHelpers.DeleteTasks();
    }
    #endregion

    #region Run Tasks
    [RelayCommand]
    private static void RunTasks()
    {
        TaskHelpers.RunTask(MainPage.Instance!.DataGridTasks);
        System.Threading.Tasks.Task.Delay(100).Wait();
        RefreshGrid();
    }
    #endregion Run Tasks

    #region Open Choose Columns in Settings
    [RelayCommand]
    private static void ChooseColumns()
    {
        _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.Settings);
        TempSettings.Setting!.AppExpanderOpen = false;
        TempSettings.Setting.ColumnsExpanderOpen = true;
        TempSettings.Setting.LangExpanderOpen = false;
        TempSettings.Setting.UIExpanderOpen = false;
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
                        break;
                    case AddTasksViewModel:
                        Views.AddTasks.Instance!.AllTasksGrid.SelectedIndex = -1;
                        break;
                }

                e.Handled = true;
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
                case Key.N:
                    _mainWindow!.NavigationListBox.SelectedValue = FindNavPage(NavPage.AddTasks);
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
                case Key.R:
                    if (UserSettings.Setting?.RowSpacing >= Spacing.Wide)
                    {
                        UserSettings.Setting.RowSpacing = Spacing.Compact;
                    }
                    else
                    {
                        UserSettings.Setting!.RowSpacing++;
                    }
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
                                UserSettings.Setting.UITheme = ThemeType.DarkBlue;
                                break;
                            case ThemeType.DarkBlue:
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
    private static void RightMouseUp(MouseButtonEventArgs e)
    {
        try
        {
            // Skip if not a text block
            if (e.OriginalSource is not TextBlock text)
            {
                return;
            }

            // Skip the DataGrid of tasks since it has a context menu that uses right-click
            DataGrid? dg = MainWindowHelpers.FindParent<DataGrid?>(text);
            if (dg is { Name: "DataGridTasks" })
            {
                return;
            }

            // Try copy to clipboard
            if (!ClipboardHelper.CopyTextToClipboard(text.Text))
            {
                return;
            }

            // Display snackbar message
            SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_CopiedToClipboardItem"));
            _log.Debug($"{text.Text.Length} bytes copied to the clipboard");
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Right-click event handler failed. {ex.Message}");
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
