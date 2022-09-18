// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks;

public partial class MainWindow : Window
{
    #region Stopwatch
    private readonly Stopwatch stopwatch = new();
    #endregion Stopwatch

    #region NLog Instance
    private static readonly Logger log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region MainWindow Instance
    internal static MainWindow Instance { get; private set; }
    #endregion MainWindow Instance

    #region File names
    private static readonly string tasksFile = Path.Combine(AppInfo.AppDirectory, "MyTasks.json");
    private static readonly string colsFile = Path.Combine(AppInfo.AppDirectory, "MyColumns.json");
    #endregion File names

    public MainWindow()
    {
        InitializeSettings();

        InitializeComponent();

        ReadSettings();

        ReadMyTasks();

        LoadData();

        ProcessCommandLine();
    }

    #region Settings
    /// <summary>
    /// Read and apply settings
    /// </summary>
    private void InitializeSettings()
    {
        stopwatch.Start();

        UserSettings.Init(UserSettings.AppFolder, UserSettings.DefaultFilename, true);

        Instance = this;
    }

    public void ReadSettings()
    {
        // Set NLog configuration
        NLHelpers.NLogConfig(UserSettings.Setting.NewLog);

        // Unhandled exception handler
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        // Put version number in window title
        WindowTitleVersionAdmin();

        // Log the version, build date and commit id
        log.Info($"{AppInfo.AppName} ({AppInfo.AppProduct}) {AppInfo.AppVersion} is starting up");
        log.Info($"{AppInfo.AppName} {AppInfo.AppCopyright}");
        log.Debug($"{AppInfo.AppName} Build date: {BuildInfo.BuildDateUtc.ToUniversalTime():f} (UTC)");
        log.Debug($"{AppInfo.AppName} Commit ID: {BuildInfo.CommitIDString} ");

        if (IsAdministrator())
        {
            log.Info($"{AppInfo.AppPath} is running as Administrator");
            mnuRestart.IsEnabled = false;
        }

        // Log the .NET version, app framework and OS platform
        string version = Environment.Version.ToString();
        log.Debug($".NET version: {AppInfo.RuntimeVersion.Replace(".NET", "")}  ({version})");
        log.Debug(AppInfo.Framework);
        log.Debug(AppInfo.OsPlatform);

        // Window position
        UserSettings.Setting.SetWindowPos();
        Topmost = UserSettings.Setting.KeepOnTop;

        // Light or dark
        MainWindowUIHelpers.SetBaseTheme((ThemeType)UserSettings.Setting.DarkMode);

        // Primary color
        MainWindowUIHelpers.SetPrimaryColor((AccentColor)UserSettings.Setting.PrimaryColor);

        // UI size
        double size = MainWindowUIHelpers.UIScale((MySize)UserSettings.Setting.UISize);
        OuterGrid.LayoutTransform = new ScaleTransform(size, size);

        //Grid row height
        SetRowSpacing((Spacing)UserSettings.Setting.RowSpacing);

        // Details pane
        detailsRow.Height = !UserSettings.Setting.ShowDetails
            ? new GridLength(1)
            : new GridLength(UserSettings.Setting.DetailsHeight);

        // Settings change event
        UserSettings.Setting.PropertyChanged += UserSettingChanged;
    }
    #endregion Settings

    #region Setting change
    /// <summary>
    /// My way of handling changes in UserSettings
    /// </summary>
    /// <param name="sender"></param>
    private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
        object newValue = prop?.GetValue(sender, null);
        log.Debug($"Setting change: {e.PropertyName} New Value: {newValue}");
        switch (e.PropertyName)
        {
            case nameof(UserSettings.Setting.KeepOnTop):
                Topmost = (bool)newValue;
                break;

            case nameof(UserSettings.Setting.IncludeDebug):
                NLHelpers.SetLogLevel((bool)newValue);
                break;

            case nameof(UserSettings.Setting.DarkMode):
                MainWindowUIHelpers.SetBaseTheme((ThemeType)newValue);
                break;

            case nameof(UserSettings.Setting.PrimaryColor):
                MainWindowUIHelpers.SetPrimaryColor((AccentColor)newValue);
                break;

            case nameof(UserSettings.Setting.UISize):
                int size = (int)newValue;
                double newSize = MainWindowUIHelpers.UIScale((MySize)size);
                OuterGrid.LayoutTransform = new ScaleTransform(newSize, newSize);
                break;

            case nameof(UserSettings.Setting.RowSpacing):
                SetRowSpacing((Spacing)newValue);
                break;

            case nameof(UserSettings.Setting.ShowDetails):
                if ((bool)newValue)
                {
                    detailsRow.Height = new GridLength(UserSettings.Setting.DetailsHeight);
                    splitter.Visibility = Visibility.Visible;
                }
                else
                {
                    detailsRow.Height = new GridLength(1);
                    splitter.Visibility = Visibility.Collapsed;
                }
                break;
        }
    }
    #endregion Setting change

    #region Command line arguments
    private void ProcessCommandLine()
    {
        // Since this is not a console app, get the command line args
        string[] args = Environment.GetCommandLineArgs();

        // Parser settings
        Parser parser = new(s =>
        {
            s.CaseSensitive = false;
            s.IgnoreUnknownArguments = true;
        });

        // parses the command line. result object will hold the arguments
        ParserResult<CmdLineOptions> result = parser.ParseArguments<CmdLineOptions>(args);

        // Check options
        if (result?.Value.Hide == true)
        {
            log.Debug("Argument \"hide\" specified. Scheduled tasks will be checked but window will only be shown if needed.");
            // hide the window
            Visibility = Visibility.Hidden;
            bool showMainWindow = false;

            // Only write so log file when the window is hidden
            foreach (var task in ScheduledTask.TaskList)
            {
                log.Debug($"{task.TaskName} result = {task.TaskResultShort}");
                if (task.IsChecked && (task.TaskResultShort == "NZ" || task.TaskResultShort == "FNF"))
                {
                    showMainWindow = true;
                    log.Info($"Last result for {task.TaskName} was {task.TaskResultHex}, will show window");
                }
            }
            // If showMainWindow is false, then shut down
            if (showMainWindow)
            {
                Visibility = Visibility.Visible;
                if (UserSettings.Setting.Sound)
                {
                    SystemSounds.Beep.Play();
                }
            }
            else
            {
                log.Info("No checked scheduled tasks with a non-zero results were found.");
                Application.Current.Shutdown();
            }
        }
        else if (result?.Value.Administrator == true)
        {
            log.Debug("Argument \"administrator\" specified. Restarting as administrator.");
            RestartAsAdmin();
        }
    }
    #endregion Command line arguments

    #region Navigation
    /// <summary>
    /// Navigate to one of the dialogs
    /// </summary>
    /// <param name="selectedIndex">Index or <see cref="Enum"/> of selected dialog</param>
    private void NavigateToPage(NavPage selectedIndex)
    {
        switch (selectedIndex)
        {
            case NavPage.TasksView:
                NavDrawer.IsLeftDrawerOpen = false;
                break;

            case NavPage.Settings:
                NavDrawer.IsLeftDrawerOpen = false;
                DialogHelpers.ShowSettingsDialog();
                break;

            case NavPage.About:
                NavDrawer.IsLeftDrawerOpen = false;
                DialogHelpers.ShowAboutDialog();
                break;

            case NavPage.TaskScheduler:
                NavDrawer.IsLeftDrawerOpen = false;
                using (Process taskSched = new())
                {
                    taskSched.StartInfo.FileName = "mmc.exe";
                    taskSched.StartInfo.Arguments = @"c:\windows\system32\taskschd.msc";
                    taskSched.StartInfo.UseShellExecute = true;
                    taskSched.Start();
                }
                break;

            case NavPage.Restart:
                NavDrawer.IsLeftDrawerOpen = false;
                QueryRestartAsAdmin();
                break;

            case NavPage.Exit:
                NavDrawer.IsLeftDrawerOpen = false;
                Close();
                break;
        }
    }
    private void NavListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        NavigateToPage((NavPage)NavListBox.SelectedIndex);
        NavListBox.SelectedItem = null;
    }

    private void NavListBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
        NavDrawer.IsLeftDrawerOpen = false;
    }
    #endregion Navigation

    #region Window Events
    private void Window_ContentRendered(object sender, EventArgs e)
    {
        CheckEmptyList();
        SetDGColumnSort();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        // Save if task list has been changed
        if (MyTasks.IsDirty)
        {
            if (UserSettings.Setting.SaveOnExit)
            {
                UpdateMyTasksCollection();
                WriteTasks2Json();
            }
            else
            {
                _ = new MDCustMsgBox("The Task List has changed.\n" +
                           "Do you wan to save the changes before exiting? \n\n" +
                           "Click YES to save changes and exit.\n" +
                           "Click NO to exit without saving.\n" +
                           "Click CANCEL to return to the application.",
                           "SAVE CHANGES?", ButtonType.YesNoCancel, true).ShowDialog();
                switch (MDCustMsgBox.CustResult)
                {
                    case CustResultType.Yes:
                        UpdateMyTasksCollection();
                        WriteTasks2Json();
                        break;

                    case CustResultType.No:
                        break;

                    case CustResultType.Cancel:
                        e.Cancel = true;
                        return;
                }
            }
        }

        if (MyTasks.SortIsDirty)
        {
            SaveDGColumnSort();
        }

        // Stop the stopwatch and record elapsed time
        stopwatch.Stop();
        log.Info($"{AppInfo.AppName} is shutting down.  Elapsed time: {stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");

        // Shut down NLog
        LogManager.Shutdown();

        // Save settings
        UserSettings.Setting.SaveWindowPos();
        UserSettings.SaveSettings();
    }
    #endregion Window Events

    #region Keyboard Events
    /// <summary>
    /// Keyboard events
    /// </summary>
    private void Window_PreviewKeydown(object sender, KeyEventArgs e)
    {
        Debug.WriteLine(e.Key);
        // CTRL key combos
        if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
        {
            if (e.Key == Key.D)
            {
                ToggleDetails();
            }
            if (e.Key == Key.E)
            {
                foreach (var item in DataGridTasks.SelectedItems)
                {
                    string taskPath = (item as ScheduledTask)?.TaskPath;
                    ExportTask(taskPath);
                }
            }
            if (e.Key == Key.N)
            {
                if (!DialogHost.IsDialogOpen("MainDialogHost"))
                {
                    DialogHelpers.ShowAddTasksDialog();
                }
                else
                {
                    DialogHost.Close("MainDialogHost");
                    DialogHelpers.ShowAddTasksDialog();
                }
            }
            if (e.Key == Key.R)
            {
                ResetCols();
            }
            if (e.Key == Key.S)
            {
                UpdateMyTasksCollection();
                WriteTasks2Json();
            }
            if (e.Key == Key.Add)
            {
                EverythingLarger();
            }
            if (e.Key == Key.Subtract)
            {
                EverythingSmaller();
            }
            if (e.Key == Key.OemComma)
            {
                if (!DialogHost.IsDialogOpen("MainDialogHost"))
                {
                    DialogHelpers.ShowSettingsDialog();
                }
                else
                {
                    DialogHost.Close("MainDialogHost");
                    DialogHelpers.ShowSettingsDialog();
                }
            }
        }

        // Ctrl and Shift
        if (e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
        {
            if (e.Key == Key.M)
            {
                switch (UserSettings.Setting.DarkMode)
                {
                    case (int)ThemeType.Light:
                        UserSettings.Setting.DarkMode = (int)ThemeType.Dark;
                        break;
                    case (int)ThemeType.Dark:
                        UserSettings.Setting.DarkMode = (int)ThemeType.Darker;
                        break;
                    case (int)ThemeType.Darker:
                        UserSettings.Setting.DarkMode = (int)ThemeType.System;
                        break;
                    case (int)ThemeType.System:
                        UserSettings.Setting.DarkMode = (int)ThemeType.Light;
                        break;
                }
                SnackbarMsg.ClearAndQueueMessage($"Theme set to {(ThemeType)UserSettings.Setting.DarkMode}");
            }
            if (e.Key == Key.N)
            {
                if (UserSettings.Setting.PrimaryColor >= (int)AccentColor.BlueGray)
                {
                    UserSettings.Setting.PrimaryColor = 0;
                }
                else
                {
                    UserSettings.Setting.PrimaryColor++;
                }
                SnackbarMsg.ClearAndQueueMessage($"Accent color set to {(AccentColor)UserSettings.Setting.PrimaryColor}");
            }
            if (e.Key == Key.R)
            {
                string readme = Path.Combine(AppInfo.AppDirectory, "ReadMe.txt");
                TextFileViewer.ViewTextFile(readme);
            }
        }

        // No CTRL or Shift key
        if (e.Key == Key.F1)
        {
            if (!DialogHost.IsDialogOpen("MainDialogHost"))
            {
                DialogHelpers.ShowAboutDialog();
            }
            else
            {
                DialogHost.Close("MainDialogHost");
                DialogHelpers.ShowAboutDialog();
            }
        }
        if (e.Key == Key.F5)
        {
            RefreshData();
        }
        if (e.Key == Key.Delete)
        {
            RemoveTasks();
        }
        if (e.Key == Key.Escape)
        {
            DataGridTasks.SelectedValue = null;
        }
    }
    #endregion Keyboard Events

    #region Buttons in the PopupBox menu
    private void BtnViewTasksFile_Click(object sender, RoutedEventArgs e)
    {
        TextFileViewer.ViewTextFile(tasksFile);
    }

    private void BtnViewLog_Click(object sender, RoutedEventArgs e)
    {
        TextFileViewer.ViewTextFile(NLHelpers.GetlogtempName());
    }

    private void BtnReadme_Click(object sender, RoutedEventArgs e)
    {
        string dir = AppInfo.AppDirectory;
        TextFileViewer.ViewTextFile(Path.Combine(dir, "ReadMe.txt"));
    }

    private void BtnRefresh_Click(object sender, RoutedEventArgs e)
    {
        RefreshData();
    }
    #endregion Buttons in the PopupBox menu

    #region Smaller/Larger
    /// <summary>
    /// Scale the UI according to user preference
    /// </summary>
    public void EverythingSmaller()
    {
        int size = UserSettings.Setting.UISize;
        if (size > 0)
        {
            size--;
            UserSettings.Setting.UISize = size;
            double newSize = MainWindowUIHelpers.UIScale((MySize)size);
            LayoutTransform = new ScaleTransform(newSize, newSize);
            SnackbarMsg.ClearAndQueueMessage($"Size set to {(MySize)UserSettings.Setting.UISize}");
        }
    }
    public void EverythingLarger()
    {
        int size = UserSettings.Setting.UISize;
        if (size < 4)
        {
            size++;
            UserSettings.Setting.UISize = size;
            double newSize = MainWindowUIHelpers.UIScale((MySize)size);
            LayoutTransform = new ScaleTransform(newSize, newSize);
            SnackbarMsg.ClearAndQueueMessage($"Size set to {(MySize)UserSettings.Setting.UISize}");
        }
    }

    private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Delta > 0)
            {
                EverythingLarger();
            }
            else if (e.Delta < 0)
            {
                EverythingSmaller();
            }
            Debug.WriteLine($"Window {e.OriginalSource}");
            e.Handled = true;
        }
    }
    #endregion Smaller/Larger

    #region Unhandled Exception Handler
    /// <summary>
    /// Handles any exceptions that weren't caught by a try-catch statement
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        log.Error("Unhandled Exception");
        Exception e = (Exception)args.ExceptionObject;
        log.Error(e.Message);
        if (e.InnerException != null)
        {
            log.Error(e.InnerException.ToString());
        }
        log.Error(e.StackTrace);

        _ = new MDCustMsgBox("An error has occurred. See the log file",
            "ERROR", ButtonType.Ok).ShowDialog();
    }
    #endregion Unhandled Exception Handler

    #region Window Title
    /// <summary>
    /// Puts the version number in the title bar as well as Administrator if running elevated
    /// </summary>
    public void WindowTitleVersionAdmin()
    {
        // Set the windows title
        if (IsAdministrator())
        {
            Title = AppInfo.AppName + " - " + AppInfo.TitleVersion + " - (Administrator)";
        }
        else
        {
            Title = AppInfo.AppName + " - " + AppInfo.TitleVersion;
        }
    }
    #endregion Window Title

    #region Running as Administrator?
    /// <summary>
    /// Determines if running as administrator (elevated)
    /// </summary>
    /// <returns>True if running elevated</returns>
    public static bool IsAdministrator()
    {
        return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
    #endregion Running as Administrator?

    #region Restart as Administrator
    /// <summary>
    /// Restart as Administrator
    /// </summary>
    private static void RestartAsAdmin()
    {
        using Process p = new();
        p.StartInfo.FileName = AppInfo.AppPath;
        p.StartInfo.UseShellExecute = true;
        p.StartInfo.Verb = "runas";
        p.Start();
        Application.Current.Shutdown();
    }

    /// <summary>
    /// Confirm restart
    /// </summary>
    private void QueryRestartAsAdmin()
    {
        MDCustMsgBox mbox = new("Restart with Elevated Permissions?",
                            "Restart as Administrator?",
                            ButtonType.YesNo,
                            false,
                            true,
                            this,
                            false);
        _ = mbox.ShowDialog();

        if (MDCustMsgBox.CustResult == CustResultType.Yes)
        {
            log.Info($"{AppInfo.AppName} is restarting as Administrator");
            RestartAsAdmin();
        }
    }
    #endregion Restart as Administrator

    #region Datagrid row double-click handler
    public void RowDoubleClick(object sender, RoutedEventArgs e)
    {
        var row = (DataGridRow)sender;
        row.DetailsVisibility = row.DetailsVisibility == Visibility.Collapsed ?
            Visibility.Visible : Visibility.Collapsed;
    }
    #endregion Datagrid row double-click handler

    #region Menu selection events
    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        RefreshData();
    }

    private void ResetCols_Click(object sender, RoutedEventArgs e)
    {
        ResetCols();
    }

    private void Add_Click(object sender, RoutedEventArgs e)
    {
        DialogHelpers.ShowAddTasksDialog();
    }

    private void Remove_Click(object sender, RoutedEventArgs e)
    {
        RemoveTasks();
    }

    private void MnuExport_Click(object sender, RoutedEventArgs e)
    {
        foreach (var item in DataGridTasks.SelectedItems)
        {
            string taskPath = (item as ScheduledTask)?.TaskPath;
            ExportTask(taskPath);
        }
    }

    private void Note_Click(object sender, RoutedEventArgs e)
    {
        if (DataGridTasks.SelectedItems.Count == 1)
        {
            var row = DataGridTasks.SelectedItem as ScheduledTask;
            DialogHelpers.ShowEditNoteDialog(row);
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        UpdateMyTasksCollection();
        WriteTasks2Json();
    }

    private void MnuExit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void EnableTask_Click(object sender, RoutedEventArgs e)
    {
        if (DataGridTasks.SelectedItems.Count == 1)
        {
            var row = DataGridTasks.SelectedItem as ScheduledTask;
            string taskPath = row.TaskPath;
            EnableTask(taskPath);
        }
    }

    private void DisableTask_Click(object sender, RoutedEventArgs e)
    {
        if (DataGridTasks.SelectedItems.Count == 1)
        {
            var row = DataGridTasks.SelectedItem as ScheduledTask;
            string taskPath = row.TaskPath;
            DisableTask(taskPath);
        }
    }

    private void MnuRunTask_Click(object sender, RoutedEventArgs e)
    {
        if (DataGridTasks.SelectedItems.Count == 1)
        {
            var row = DataGridTasks.SelectedItem as ScheduledTask;
            string taskPath = row.TaskPath;
            RunTask(taskPath);
        }
    }

    private void OpenReadme_Click(object sender, RoutedEventArgs e)
    {
        string readme = Path.Combine(AppInfo.AppDirectory, "ReadMe.txt");
        TextFileViewer.ViewTextFile(readme);
    }

    private void OpenAbout_Click(object sender, RoutedEventArgs e)
    {
        DialogHelpers.ShowAboutDialog();
    }

    private void ToggleDetails_Click(object sender, RoutedEventArgs e)
    {
        ToggleDetails();
    }

    private void MnuLarger_Click(object sender, RoutedEventArgs e)
    {
        EverythingLarger();
    }

    private void MnuSmaller_Click(object sender, RoutedEventArgs e)
    {
        EverythingSmaller();
    }
    #endregion Menu selection events

    #region Disable, Enable, Run and Remove tasks
    private void DisableTask(string taskName)
    {
        using TaskService ts = new();
        Task task = ts.GetTask(taskName);
        if (task == null)
            return;
        try
        {
            task.Enabled = false;
            RefreshData();
            SnackbarMsg.ClearAndQueueMessage($"Disabled: {task.Name}");
            log.Info($"Disabled {task.Path}");
        }
        catch (Exception ex)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage($"Error attempting to disable {task.Name}", 5000);
            log.Error(ex, $"Error attempting to disable {task.Name}");
        }
    }

    private void EnableTask(string taskName)
    {
        using TaskService ts = new();
        Task task = ts.GetTask(taskName);
        if (task == null)
            return;
        try
        {
            task.Enabled = true;
            RefreshData();
            SnackbarMsg.ClearAndQueueMessage($"Enabled: {task.Name}");
            log.Info($"Enabled {task.Path}");
        }
        catch (Exception ex)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage($"Error attempting to enable {task.Name}", 5000);
            log.Error(ex, $"Error attempting to enable {task.Name}");
        }
    }

    private void RunTask(string taskName)
    {
        using TaskService ts = new();
        Task task = ts.GetTask(taskName);
        if (task == null)
            return;
        try
        {
            task.Run();
            RefreshData();
            SnackbarMsg.ClearAndQueueMessage($"Running: {task.Name}");
            log.Info($"Running {task.Path}");
        }
        catch (Exception ex)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage($"Error attempting to run {task.Name}. See the log file for details.", 5000);
            log.Error(ex, $"Error attempting to run {task.Name}");
        }
    }

    private static void ExportTask(string taskName)
    {
        using TaskService ts = new();
        Task task = ts.GetTask(taskName);
        if (task == null)
            return;
        try
        {
            SaveFileDialog dialog = new()
            {
                Title = "Export Task",
                Filter = "XML File|*.xml",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                FileName = task.Name + ".xml"
            };
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                task.Export(dialog.FileName);
                SnackbarMsg.ClearAndQueueMessage($"Exported: {task.Name}");
                log.Info($"Exported {task.Path}");
            }
        }
        catch (Exception ex)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage($"Error attempting to export {task.Name}", 5000);
            log.Error(ex, $"Error attempting to export {task.Path}");
        }
    }

    private void RemoveTasks()
    {
        if (DataGridTasks.SelectedItems.Count == 0)
        {
            return;
        }

        if (DataGridTasks.SelectedItems.Count <= 5)
        {
            for (int i = DataGridTasks.SelectedItems.Count - 1; i >= 0; i--)
            {
                var row = (ScheduledTask)DataGridTasks.SelectedItems[i];
                ScheduledTask.TaskList.Remove(row);
                log.Info($"Removed \"{row.TaskPath}\"");
                SnackbarMsg.QueueMessage($"Removed {row.TaskName}", 1000);
            }
        }
        else if (DataGridTasks.SelectedItems.Count > 5)
        {
            int count = DataGridTasks.SelectedItems.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                var row = (ScheduledTask)DataGridTasks.SelectedItems[i];
                ScheduledTask.TaskList.Remove(row);
                log.Info($"Removed \"{row.TaskPath}\"");
            }
            SnackbarMsg.QueueMessage($"Removed {count} tasks", 2000);
        }
    }
    #endregion Disable, Enable and Run tasks

    #region Menu open events
    private void MenuOpened(object sender, RoutedEventArgs e)
    {
        ctxEnable.IsEnabled = true;
        ctxDisable.IsEnabled = true;
        ctxRunTask.IsEnabled = true;
        ctxExport.IsEnabled = true;
        ctxEditNote.IsEnabled = true;
        ctxRemove.IsEnabled = true;
        mnuRemove.IsEnabled = true;
        mnuExport.IsEnabled = true;
        mnuEnable.IsEnabled = true;
        mnuDisable.IsEnabled = true;

        if (!IsAdministrator())
        {
            ctxEnable.IsEnabled = false;
            ctxDisable.IsEnabled = false;
            ctxRunTask.IsEnabled = false;
            mnuEnable.IsEnabled = false;
            mnuDisable.IsEnabled = false;
            mnuRunTask.IsEnabled = false;
        }
        if (DataGridTasks.SelectedItems.Count == 0)
        {
            ctxEnable.IsEnabled = false;
            ctxDisable.IsEnabled = false;
            ctxRunTask.IsEnabled = false;
            ctxExport.IsEnabled = false;
            ctxEditNote.IsEnabled = false;
            ctxRemove.IsEnabled = false;
            mnuRemove.IsEnabled = false;
            mnuExport.IsEnabled = false;
            mnuEnable.IsEnabled = false;
            mnuDisable.IsEnabled = false;
        }
        if (DataGridTasks.SelectedItems.Count > 1)
        {
            ctxEditNote.IsEnabled = false;
            ctxEnable.IsEnabled = false;
            ctxDisable.IsEnabled = false;
            ctxRunTask.IsEnabled = false;
            mnuEnable.IsEnabled = false;
            mnuDisable.IsEnabled = false;
            mnuRunTask.IsEnabled = false;
        }
    }
    #endregion Menu open events

    #region Refresh data
    internal void RefreshData()
    {
        if (MyTasks.IsDirty)
        {
            UpdateMyTasksCollection();
            WriteTasks2Json();
            ReadMyTasks();
        }
        ScheduledTask.TaskList.Clear();
        LoadData();
        DataGridTasks.ItemsSource = ScheduledTask.TaskList;

        // MyTasks.MyTasksCollection isn't really dirty, it's been reloaded with the same data
        // so we reset the dirty flag and turn off the warning message in the status bar.
        MyTasks.IsDirty = false;
        sbRight.Content = string.Empty;
    }
    #endregion Refresh data

    #region Save datagrid columns sort
    /// <summary>
    /// Saves the sort order of the datagrid columns
    /// </summary>
    public void SaveDGColumnSort()
    {
        List<ColumnSort> csList = new();
        foreach (DataGridColumn col in DataGridTasks.Columns)
        {
            if (col.SortDirection != null)
            {
                csList.Add(new ColumnSort
                {
                    Header = col.Header.ToString(),
                    Path = col.SortMemberPath,
                    SortDirection = col.SortDirection.ToString()
                });
            }
            else
            {
                csList.Add(new ColumnSort
                {
                    Header = col.Header.ToString(),
                    Path = col.SortMemberPath,
                    SortDirection = null
                });
            }
            string dir = col.SortDirection != null ? col.SortDirection.ToString() : "null";
            log.Debug($"Saving column \"{col.Header}\" {col.SortMemberPath} sort is {dir}");
        }

        try
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(csList, options);
            File.WriteAllText(colsFile, json);
        }
        catch (Exception ex)
        {
            log.Error(ex, $"Error saving {colsFile}");
            if (!DialogHost.IsDialogOpen("MainDialogHost"))
            {
                DialogHelpers.ShowErrorDialog($"Error saving {colsFile}\n\n{ex.Message}");
            }
            else
            {
                DialogHost.Close("MainDialogHost");
                DialogHelpers.ShowErrorDialog($"Error saving {colsFile}\n\n{ex.Message}");
            }
        }
    }
    #endregion Save datagrid columns sort

    #region Set datagrid column sort
    /// <summary>
    /// Reads "colsFile" and then applies sort order to datagrid columns.
    /// </summary>
    public void SetDGColumnSort()
    {
        if (File.Exists(colsFile))
        {
            DataGridTasks.Items.SortDescriptions.Clear();
            string json = File.ReadAllText(colsFile);
            var sortList = JsonSerializer.Deserialize<List<ColumnSort>>(json);
            for (int i = 0; i < sortList.Count; i++)
            {
                ColumnSort item = sortList[i];
                DataGridColumn column = DataGridTasks.Columns[i];
                if (item.SortDirection == "Ascending")
                {
                    DataGridTasks.Items.SortDescriptions.Add(new SortDescription(
                        column.SortMemberPath, ListSortDirection.Ascending));
                    column.SortDirection = ListSortDirection.Ascending;
                    log.Info($"Column \"{column.Header}\" {column.SortMemberPath} sort is Ascending");
                }
                else if (item.SortDirection == "Descending")
                {
                    DataGridTasks.Items.SortDescriptions.Add(new SortDescription(
                        column.SortMemberPath, ListSortDirection.Descending));
                    column.SortDirection = ListSortDirection.Descending;
                    log.Info($"Column \"{column.Header}\" {column.SortMemberPath} sort is Descending");
                }
                else
                {
                    column.SortDirection = null;
                }
            }
            DataGridTasks.Items.Refresh();
        }
    }
    #endregion Set datagrid column sort

    #region Datagrid sorting event
    private void DataGridTasks_Sorting(object sender, DataGridSortingEventArgs e)
    {
        MyTasks.SortIsDirty = true;

        log.Debug($"DataGrid sorting event: {e.Column.Header} (Index: {e.Column.DisplayIndex})  {e.Column.SortMemberPath}");
    }
    #endregion Datagrid sorting event

    #region Get updated tasks from TaskList
    public static void UpdateMyTasksCollection()
    {
        MyTasks.MyTasksCollection.Clear();
        foreach (ScheduledTask item in ScheduledTask.TaskList)
        {
            if (item.TaskPath != null)
            {
                MyTasks.MyTasksCollection.Add(new MyTasks(item.TaskPath, item.IsChecked, item.TaskNote));
            }
        }
    }
    #endregion Get updated tasks from TaskList

    #region Write the tasks JSON file
    /// <summary>
    /// Convert MyTasksCollection to JSON and save it to a file
    /// </summary>
    public void WriteTasks2Json()
    {
        try
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };
            string x = JsonSerializer.Serialize(MyTasks.MyTasksCollection, options);
            File.WriteAllText(tasksFile, x);
            log.Info($"Writing {MyTasks.MyTasksCollection.Count} items to {tasksFile} ");
            SnackbarMsg.ClearAndQueueMessage($"{tasksFile} has been saved");
            MyTasks.IsDirty = false;
            sbRight.Content = string.Empty;
        }
        catch (Exception ex)
        {
            log.Error(ex, $"Error saving {tasksFile}");
            if (!DialogHost.IsDialogOpen("MainDialogHost"))
            {
                DialogHelpers.ShowErrorDialog($"Error saving {tasksFile}\n\n{ex.Message}");
            }
            else
            {
                DialogHost.Close("MainDialogHost");
                DialogHelpers.ShowErrorDialog($"Error saving {tasksFile}\n\n{ex.Message}");
            }
        }
    }
    #endregion Write the tasks JSON file

    #region Read the tasks JSON file
    private static void ReadMyTasks()
    {
        // If the file doesn't exist, create a minimal JSON file
        if (!File.Exists(tasksFile))
        {
            const string x = "[]";
            try
            {
                File.WriteAllText(tasksFile, x);
            }
            catch (Exception ex)
            {
                log.Fatal(ex, $"Error creating {tasksFile}");
                _ = new MDCustMsgBox($"Error creating {tasksFile}\n\n{ex.Message}",
                                    "ERROR", ButtonType.Ok).ShowDialog();

                _ = new MDCustMsgBox("Fatal Error. My Scheduled Tasks will now close.",
                                    "FATAL ERROR", ButtonType.Ok, true).ShowDialog();

                // Quit via Environment.Exit so that normal shutdown processing doesn't run
                Environment.Exit(1);
            }
        }

        // Read the JSON file
        try
        {
            string json = File.ReadAllText(tasksFile);
            MyTasks.MyTasksCollection = JsonSerializer.Deserialize<ObservableCollection<MyTasks>>(json);
            log.Info($"Read {MyTasks.MyTasksCollection.Count} items from {tasksFile} ");
        }
        // Can't really do much if the file is not readable
        catch (Exception ex)
        {
            log.Fatal(ex, $"Error reading {tasksFile}");
            _ = new MDCustMsgBox($"Error reading {tasksFile}\n\n{ex.Message}",
                                "ERROR", ButtonType.Ok).ShowDialog();

            _ = new MDCustMsgBox("Fatal Error. My Scheduled Tasks will now close.",
                                "FATAL ERROR", ButtonType.Ok, true).ShowDialog();

            // Quit via Environment.Exit so that normal shutdown processing doesn't run
            Environment.Exit(1);
        }
    }
    #endregion Read the tasks JSON file

    #region Load the task list
    /// <summary>
    /// Load the task list from MyTasksCollection
    /// </summary>
    public void LoadData()
    {
        ObservableCollection<ScheduledTask> taskList = new();
        BindingList<ScheduledTask> bindingList = new();
        ScheduledTask st = new();
        int count = 0;
        foreach (MyTasks item in MyTasks.MyTasksCollection)
        {
            Task task = TaskInfo.GetTaskInfo(item.TaskPath);

            if (task != null)
            {
                ScheduledTask schedTask = ScheduledTask.BuildSchedTask(task, item);
                taskList.Add(schedTask);
                bindingList.Add(schedTask);
                count++;
                log.Debug($"Added {count,3}: \"{item.TaskPath}\"");
            }
            else if (item.TaskPath == null)
            {
                log.Warn("Null item found while reading");
                SnackbarMsg.QueueMessage($"Null item found while reading the task list in {tasksFile}.  See the log file.", 4000);
            }
            else
            {
                log.Warn($"No information found for \"{item.TaskPath}\"");
                SnackbarMsg.QueueMessage($"No information found for {item.TaskPath}.  See the log file.", 4000);
            }
        }
        DataContext = st;
        ScheduledTask.TaskList = taskList;
        ScheduledTask.TaskList.CollectionChanged += TaskList_CollectionChanged;
        bindingList.ListChanged += Binding_ListChanged;
        sbLeft.Content = ScheduledTask.TaskList.Count;
    }
    #endregion Load the task list

    #region List change events
    /// <summary>
    /// The BindingList can notify when the checkbox changes
    /// </summary>
    private void Binding_ListChanged(object sender, ListChangedEventArgs e)
    {
        if (e != null)
        {
            MyTasks.IsDirty = true;
            sbRight.Content = "Unsaved changes";
        }
    }
    /// <summary>
    /// The collection change will notify when a list item is added or deleted
    /// </summary>
    private void TaskList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        MyTasks.IsDirty = true;
        sbRight.Content = "Unsaved changes";
        sbLeft.Content = ScheduledTask.TaskList.Count;
    }
    #endregion List change events

    #region Reset datagrid column sort
    /// <summary>
    /// Reset any sorted columns
    /// </summary>
    internal void ResetCols()
    {
        foreach (var column in DataGridTasks.Columns)
        {
            column.SortDirection = null;
        }
        DataGridTasks.Items.SortDescriptions.Clear();
        SaveDGColumnSort();
    }
    #endregion Reset datagrid column sort

    #region Datagrid drop complete
    /// <summary>
    /// Drag & drop in datagrid sets MyTasks.IsDirty true
    /// </summary>
    private void DataGridTasks_Drop(object sender, DragEventArgs e)
    {
        MyTasks.IsDirty = true;
    }
    #endregion Datagrid drop complete

    #region Splitter drag complete
    private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
        UserSettings.Setting.DetailsHeight = Math.Floor(detailsRow.Height.Value);
    }
    #endregion Splitter drag complete

    #region Set the row spacing
    /// <summary>
    /// Sets the padding around the rows in the datagrid
    /// </summary>
    /// <param name="spacing"></param>
    public void SetRowSpacing(Spacing spacing)
    {
        switch (spacing)
        {
            case Spacing.Compact:
                DataGridAssist.SetCellPadding(DataGridTasks, new Thickness(15, 2, 15, 2));
                break;
            case Spacing.Comfortable:
                DataGridAssist.SetCellPadding(DataGridTasks, new Thickness(15, 4, 15, 4));
                break;
            case Spacing.Wide:
                DataGridAssist.SetCellPadding(DataGridTasks, new Thickness(15, 8, 15, 8));
                break;
        }
    }
    #endregion Set the row spacing

    #region Check for empty task list (first run)
    private static void CheckEmptyList()
    {
        if (ScheduledTask.TaskList.Count == 0)
        {
            _ = new MDCustMsgBox("The task list is empty. Would you like to add tasks now?",
                               "ADD TASKS?", ButtonType.YesNo).ShowDialog();

            if (MDCustMsgBox.CustResult == CustResultType.Yes)
            {
                DialogHelpers.ShowAddTasksDialog();
            }
        }
    }
    #endregion Check for empty task list (first run)

    #region Datagrid selection changed event
    private void DataGridTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (DataGridTasks.SelectedItems.Count == 0)
        {
            sbSelected.Content = string.Empty;
        }
        else
        {
            sbSelected.Content = $"{DataGridTasks.SelectedItems.Count} selected";
        }
    }
    #endregion Datagrid selection changed event

    #region Toggle details
    private static void ToggleDetails()
    {
        UserSettings.Setting.ShowDetails = !UserSettings.Setting.ShowDetails;
    }
    #endregion Toggle details

    #region Double click ColorZone
    /// <summary>
    /// Double click the ColorZone to set optimal width
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ColorZone_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        SizeToContent = SizeToContent.Width;
        double width = ActualWidth;
        Thread.Sleep(50);
        SizeToContent = SizeToContent.Manual;
        Width = width + 1;
    }
    #endregion Double click ColorZone
}
