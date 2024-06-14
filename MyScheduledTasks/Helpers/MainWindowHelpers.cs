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
    }
    #endregion Startup

    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region StopWatch
    public static readonly Stopwatch _stopwatch = Stopwatch.StartNew();
    #endregion StopWatch

    #region Set and Save MainWindow position and size
    /// <summary>
    /// Sets the MainWindow position and size.
    /// </summary>
    public static void SetWindowPosition()
    {
        Window mainWindow = Application.Current.MainWindow;
        mainWindow.Height = UserSettings.Setting!.WindowHeight;
        mainWindow.Left = UserSettings.Setting!.WindowLeft;
        mainWindow.Top = UserSettings.Setting!.WindowTop;
        mainWindow.Width = UserSettings.Setting!.WindowWidth;

        if (UserSettings.Setting.StartCentered)
        {
            mainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        // The following will ensure that the window appears on screen
        if (mainWindow.Top < SystemParameters.VirtualScreenTop)
        {
            mainWindow.Top = SystemParameters.VirtualScreenTop;
        }

        if (mainWindow.Left < SystemParameters.VirtualScreenLeft)
        {
            mainWindow.Left = SystemParameters.VirtualScreenLeft;
        }

        if (mainWindow.Left + mainWindow.Width > SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth)
        {
            mainWindow.Left = SystemParameters.VirtualScreenWidth + SystemParameters.VirtualScreenLeft - mainWindow.Width;
        }

        if (mainWindow.Top + mainWindow.Height > SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight)
        {
            mainWindow.Top = SystemParameters.VirtualScreenHeight + SystemParameters.VirtualScreenTop - mainWindow.Height;
        }
    }

    /// <summary>
    /// Saves the MainWindow position and size.
    /// </summary>
    public static void SaveWindowPosition()
    {
        Window mainWindow = Application.Current.MainWindow;
        UserSettings.Setting!.WindowHeight = Math.Floor(mainWindow.Height);
        UserSettings.Setting!.WindowLeft = Math.Floor(mainWindow.Left);
        UserSettings.Setting!.WindowTop = Math.Floor(mainWindow.Top);
        UserSettings.Setting!.WindowWidth = Math.Floor(mainWindow.Width);
    }
    #endregion Set and Save MainWindow position and size

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
        UserSettings.Setting!.PropertyChanged += SettingChange.UserSettingChanged!;
        TempSettings.Setting!.PropertyChanged += SettingChange.TempSettingChanged!;

        // Window closing event
        _mainWindow!.Closing += MainWindow_Closing!;

        // Window content rendered event
        _mainWindow.ContentRendered += ContentRendered!;
    }
    #endregion Event handlers

    #region Window Events
    private static void ContentRendered(object sender, EventArgs e)
    {
        _ = System.Threading.Tasks.Task.Run(TaskHelpers.GetAllTasks);
        TaskFileHelpers.CheckEmptyList();
    }

    private static void MainWindow_Closing(object sender, CancelEventArgs e)
    {
        // Save tasks file if it is "dirty"
        if (MyTasks.IsDirty)
        {
            TaskFileHelpers.WriteTasksToFile();
        }

        // Clear any remaining messages
        Snackbar snackbar = FindChild<Snackbar>(Application.Current.MainWindow, "SnackBar1");
        if (snackbar is not null)
        {
            snackbar.MessageQueue!.Clear();
        }

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
            _mainWindow!.Visibility = Visibility.Hidden;
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
                if (UserSettings.Setting!.Sound)
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

    #region Find a child of a control
    /// <summary>
    /// Finds a Child of a given item in the visual tree.
    /// </summary>
    /// <param name="parent">A direct parent of the queried item.</param>
    /// <typeparam name="T">The type of the queried item.</typeparam>
    /// <param name="childName">x:Name or Name of child. </param>
    /// <returns>The first child item that matches the submitted type parameter.</returns>
    public static T FindChild<T>(DependencyObject parent, string childName)
       where T : DependencyObject
    {
        // Confirm parent and childName are valid. 
        if (parent == null)
            return null!;

        T foundChild = null!;

        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(parent, i);

            // If the child is not of the request child type child
            if (child is not T)
            {
                // recursively drill down the tree
                foundChild = FindChild<T>(child, childName);

                // If the child is found, break so we do not overwrite the found child. 
                if (foundChild != null)
                    break;
            }
            else if (!string.IsNullOrEmpty(childName))
            {
                // If the child's name is set for search
                if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                {
                    // if the child's name is of the request name
                    foundChild = (T)child;
                    break;
                }
            }
            else
            {
                // child element found.
                foundChild = (T)child;
                break;
            }
        }
        return foundChild!;
    }
    #endregion Find a child of a control

    #region Find the parent of a control
    public static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        //get parent item
        DependencyObject parentObject = VisualTreeHelper.GetParent(child);

        //we've reached the end of the tree
        if (parentObject == null)
            return null!;

        //check if the parent matches the type we're looking for
        if (parentObject is T parent)
            return parent;
        else
            return FindParent<T>(parentObject);
    }
    #endregion Find the parent of a control
}
