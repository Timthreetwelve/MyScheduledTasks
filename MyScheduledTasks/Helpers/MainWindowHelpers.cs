// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

/// <summary>
/// Class for methods used by the MainWindow and perhaps other classes.
/// </summary>
internal static class MainWindowHelpers
{
    #region Startup
    internal static void MyScheduledTasksStartUp()
    {
        EventHandlers();

        MainWindowUIHelpers.ApplyUISettings();

        TaskFileHelpers.ReadMyTasks();

        MainViewModel.LoadData();

        ProcessCommandLine();

        _ = System.Threading.Tasks.Task.Run(TaskHelpers.GetAllTasks);
    }
    #endregion Startup

    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region StopWatch
    public static Stopwatch _stopwatch = Stopwatch.StartNew();
    #endregion StopWatch

    #region Set and Save MainWindow position and size
    /// <summary>
    /// Sets the MainWindow position and size.
    /// </summary>
    public static void SetWindowPosition()
    {
        Window mainWindow = Application.Current.MainWindow;
        mainWindow.Height = UserSettings.Setting.WindowHeight;
        mainWindow.Left = UserSettings.Setting.WindowLeft;
        mainWindow.Top = UserSettings.Setting.WindowTop;
        mainWindow.Width = UserSettings.Setting.WindowWidth;

        if (UserSettings.Setting.StartCentered)
        {
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
    }

    /// <summary>
    /// Saves the MainWindow position and size.
    /// </summary>
    public static void SaveWindowPosition()
    {
        Window mainWindow = Application.Current.MainWindow;
        UserSettings.Setting.WindowHeight = Math.Floor(mainWindow.Height);
        UserSettings.Setting.WindowLeft = Math.Floor(mainWindow.Left);
        UserSettings.Setting.WindowTop = Math.Floor(mainWindow.Top);
        UserSettings.Setting.WindowWidth = Math.Floor(mainWindow.Width);
    }
    #endregion Set and Save MainWindow position and size

    #region Get property value
    /// <summary>
    /// Gets the value of the property
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns>An object containing the value of the property</returns>
    public static object GetPropertyValue(object sender, PropertyChangedEventArgs e)
    {
        PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
        return prop?.GetValue(sender, null);
    }
    #endregion Get property value

    #region Window Title
    /// <summary>
    /// Puts the version number in the title bar as well as Administrator if running elevated
    /// </summary>
    public static string WindowTitleVersionAdmin()
    {
        // Set the windows title
        return AppInfo.IsAdmin
            ? $"{AppInfo.AppProduct}  {AppInfo.AppProductVersion} - ({GetStringResource("MsgText_WindowTitleAdministrator")})"
            : $"{AppInfo.AppProduct}  {AppInfo.AppProductVersion}";
    }
    #endregion Window Title

    #region Event handlers
    /// <summary>
    /// Event handlers.
    /// </summary>
    internal static void EventHandlers()
    {
        // Unhandled exception handler
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        // Settings change events
        UserSettings.Setting.PropertyChanged += SettingChange.UserSettingChanged;
        TempSettings.Setting.PropertyChanged += SettingChange.TempSettingChanged;

        // Window closing event
        _mainWindow.Closing += MainWindow_Closing;

        // Window content rendered event
        _mainWindow.ContentRendered += ContentRendered;
    }
    #endregion Event handlers

    #region Window Events
    private static void ContentRendered(object sender, EventArgs e)
    {
        TaskFileHelpers.CheckEmptyList();
    }

    private static void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        // Clear any remaining messages
        _mainWindow.SnackBar1.MessageQueue.Clear();

        // Stop the _stopwatch and record elapsed time
        _stopwatch.Stop();
        _log.Info($"{AppInfo.AppName} {GetStringResource("MsgText_ApplicationShutdown")}.  " +
                         $"{GetStringResource("MsgText_ElapsedTime")}: {_stopwatch.Elapsed:h\\:mm\\:ss\\.ff}");

        // Shut down NLog
        LogManager.Shutdown();

        // Save settings
        SaveWindowPosition();
        ConfigHelpers.SaveSettings();
    }
    #endregion Window Events

    #region Unhandled Exception Handler
    /// <summary>
    /// Handles any exceptions that weren't caught by a try-catch statement.
    /// </summary>
    /// <remarks>
    /// This uses default message box.
    /// </remarks>
    internal static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        _log.Error("Unhandled Exception");
        Exception e = (Exception)args.ExceptionObject;
        _log.Error(e.Message);
        if (e.InnerException != null)
        {
            _log.Error(e.InnerException.ToString());
        }
        _log.Error(e.StackTrace);

        string msg = string.Format($"{GetStringResource("MsgText_ErrorGeneral")}\n{e.Message}\n{GetStringResource("MsgText_SeeLogFile")}");
        _ = MessageBox.Show(msg,
            GetStringResource("MsgText_ErrorCaption"),
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    #endregion Unhandled Exception Handler

    #region Write startup messages to the log
    /// <summary>
    /// Initializes NLog and writes startup messages to the log.
    /// </summary>
    internal static void LogStartup()
    {
        // Set NLog configuration
        NLogConfig(false);

        // Log the version, build date and commit id
        _log.Info($"{AppInfo.AppName} ({AppInfo.AppProduct}) {AppInfo.AppProductVersion} {GetStringResource("MsgText_ApplicationStarting")}");
        _log.Info($"{AppInfo.AppName} {AppInfo.AppCopyright}");
        _log.Debug($"{AppInfo.AppName} was started from {AppInfo.AppPath}");
        _log.Debug($"{AppInfo.AppName} Build date: {BuildInfo.BuildDateStringUtc}");
        _log.Debug($"{AppInfo.AppName} Commit ID: {BuildInfo.CommitIDString}");
        if (AppInfo.IsAdmin)
        {
            _log.Debug($"{AppInfo.AppName} is running as Administrator");
        }

        // Log the .NET version and OS platform
        _log.Debug($"Operating System version: {AppInfo.OsPlatform}");
        _log.Debug($".NET version: {AppInfo.RuntimeVersion.Replace(".NET", "")}");
    }
    #endregion Write startup messages to the log

    #region Process command line options
    private static void ProcessCommandLine()
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

        if (result?.Value.Administrator == true)
        {
            _log.Debug("Command line argument \"administrator\" was specified. Restarting as Administrator.");
            NavigationViewModel.RestartAsAdmin();
        }

        if (result?.Value.Hide == true)
        {
            _log.Debug("Argument \"hide\" specified. Scheduled tasks will be checked but window will only be shown if needed.");
            // hide the window
            _mainWindow.Visibility = Visibility.Hidden;
            bool showMainWindow = false;

            // Only write so log file when the window is hidden
            foreach (ScheduledTask task in ScheduledTask.TaskList)
            {
                _log.Debug($"{task.TaskName} result = {task.TaskResultShort}");
                if (task.IsChecked && task.TaskResult != 0)
                {
                    showMainWindow = true;
                    _log.Info($"Last result for {task.TaskName} was {task.TaskResultHex}, will show window.");
                }
            }
            // If showMainWindow is false, then shut down
            if (showMainWindow)
            {
                _mainWindow.Visibility = Visibility.Visible;
                if (UserSettings.Setting.Sound)
                {
                    SystemSounds.Beep.Play();
                }
            }
            else
            {
                _log.Info("No checked scheduled tasks with a non-zero results were found.");
                Application.Current.Shutdown();
            }
        }
    }
    #endregion Process command line options
}
