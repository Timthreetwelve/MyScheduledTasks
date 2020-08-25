// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32.TaskScheduler;
using MyScheduledTasks.Properties;
using Newtonsoft.Json;
using NLog;
using NLog.Targets;
using TKUtils;
using MessageBoxImage = TKUtils.MessageBoxImage;
#endregion Using directives

namespace MyScheduledTasks
{
    public partial class MainWindow : Window
    {
        #region File names
        private readonly string tasksFile = Path.Combine(AppInfo.AppDirectory, "MyTasks.json");
        private readonly string colsFile = Path.Combine(AppInfo.AppDirectory, "MyColumns.json");
        #endregion File names

        #region Stopwatch
        private readonly Stopwatch stopwatch = new Stopwatch();
        #endregion Stopwatch

        #region NLog
        private static readonly Logger log = LogManager.GetCurrentClassLogger();
        #endregion NLog

        #region MainWindow constructor
        public MainWindow()
        {
            InitializeComponent();

            ReadSettings();

            ReadMyTasks();

            LoadData();

            ProcessCommandLine();
        }
        #endregion MainWindow constructor

        #region Read Settings
        private void ReadSettings()
        {
            // Change the log file filename when debugging
            if (Debugger.IsAttached)
                GlobalDiagnosticsContext.Set("TempOrDebug", "debug");
            else
                GlobalDiagnosticsContext.Set("TempOrDebug", "temp");

            // Startup message in the temp file
            log.Info($"{AppInfo.AppName} {AppInfo.TitleVersion} is starting up");

            if (IsAdministrator())
            {
                log.Info($"{AppInfo.AppPath} is running as Administrator");
            }

            // Start the elapsed time timer
            stopwatch.Start();

            // Unhandled exception handler
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Settings upgrade
            if (Settings.Default.SettingsUpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsUpgradeRequired = false;
                Settings.Default.Save();
                CleanUp.CleanupPrevSettings();
            }

            // Settings change event
            Settings.Default.SettingChanging += SettingChanging;

            // Max screen height slightly smaller than screen
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 20;

            // Window position
            Top = Settings.Default.WindowTop;
            Left = Settings.Default.WindowLeft;

            // Put version number in window title
            WindowTitleVersionAdmin();

            // Set Datagrid zoom
            double curZoom = Settings.Default.GridZoom;
            DataGridTasks.LayoutTransform = new ScaleTransform(curZoom, curZoom);

            // Alternate row shading
            if (Settings.Default.ShadeAltRows)
            {
                AltRowShadingOn();
            }
            log.Debug($"Read settings complete {stopwatch.Elapsed} elapsed time");
        }
        #endregion Read Settings

        #region Read the tasks JSON file
        private void ReadMyTasks()
        {
            // If the file doesn't exist, create a minimal JSON file instead of blowing up
            if (!File.Exists(tasksFile))
            {
                const string x = "[\n  {\n    \"TaskPath\": \"\\\\\",\n    \"Alert\": false\n  }  \n]";
                File.WriteAllText(tasksFile, x);
            }

            // Read the JSON file
            try
            {
                string json = File.ReadAllText(tasksFile);
                MyTasks.MyTasksCollection = JsonConvert.DeserializeObject<ObservableCollection<MyTasks>>(json);
                log.Info($"Read {MyTasks.MyTasksCollection.Count} items from {tasksFile} ");
            }
            // Can't really do much if the file is not readable
            catch (Exception ex)
            {
                log.Fatal(ex, $"Error reading {tasksFile}");
                _ = TKMessageBox.Show($"Error reading {tasksFile}\n\n{ex.Message}",
                                    "Error!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                _ = TKMessageBox.Show($"Unrecoverable Error!\n\n{AppInfo.AppName} will now exit.",
                                    "Unrecoverable Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);

                // Quit via Environment.Exit so that normal shutdown processing doesn't run
                Environment.Exit(1);
            }
        }
        #endregion Read the tasks JSON file

        #region Load the Datagrid
        public void LoadData()
        {
            ObservableCollection<ScheduledTask> taskList = new ObservableCollection<ScheduledTask>();
            BindingList<ScheduledTask> bindingList = new BindingList<ScheduledTask>();
            ScheduledTask st = new ScheduledTask();
            foreach (MyTasks item in MyTasks.MyTasksCollection)
            {
                Task task = GetTaskInfo(item.TaskPath);

                if (task != null)
                {
                    // **************************************************************************
                    // if changes are made to the following, change the corresponding code in the
                    // AddSelectWindow.AddSelected items method.
                    // **************************************************************************
                    string folder = item.TaskPath.Replace(task.Name, "");
                    if (folder == "\\")
                    {
                        folder = "\\  (root)";
                    }
                    ScheduledTask schedTask = new ScheduledTask
                    {
                        TaskName = task.Name,
                        TaskStatus = task.State.ToString(),
                        TaskResult = task.LastTaskResult,
                        LastRun = task.LastRunTime,
                        NextRun = task.NextRunTime,
                        TaskPath = task.Path,
                        TaskFolder = folder,
                        TaskMissedRuns = task.NumberOfMissedRuns,
                        TaskAccount = task.Definition.Principal.Account,
                        TaskRunLevel = task.Definition.Principal.RunLevel.ToString(),
                        TaskDescription = task.Definition.RegistrationInfo.Description,
                        TaskAuthor = task.Definition.RegistrationInfo.Author,
                        TaskTriggers = task.Definition.Triggers.ToString(),
                        IsChecked = item.Alert,
                        TaskNote = item.TaskNote
                    };
                    taskList.Add(schedTask);
                    bindingList.Add(schedTask);
                    log.Debug($"Read \"{item.TaskPath}\"");
                }
                else
                {
                    if (item.TaskPath == null)
                    {
                        log.Warn("Null item found while reading");
                    }
                    else
                    {
                        log.Warn($"No information found for \"{item.TaskPath}\"");
                    }
                }
            }
            DataContext = st;
            ScheduledTask.TaskList = taskList;
            ScheduledTask.TaskList.CollectionChanged += TaskList_CollectionChanged;
            bindingList.ListChanged += Binding_ListChanged;
            sbLeft.Content = ScheduledTask.TaskList.Count;
        }
        #endregion Load the Datagrid

        #region Command line arguments
        private void ProcessCommandLine()
        {
            // If count is less that two, bail out
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2)
                return;

            foreach (string item in args)
            {
                // If command line argument "hide" is found, execute without showing window.
                string arg = item.Replace("-", "").Replace("/", "").ToLower();

                if (arg == "hide")
                {
                    log.Info($"Command line argument \"{item}\" found.");

                    // hide the window
                    Visibility = Visibility.Hidden;

                    bool showMainWindow = false;

                    // Only write so log file when the window is hidden
                    foreach (var task in ScheduledTask.TaskList)
                    {
                        log.Debug($"{task.TaskName} checked = {task.IsChecked} result = {task.TaskResultShort}");
                        if (task.IsChecked && task.TaskResultShort == "NZ")
                        {
                            Visibility = Visibility.Visible;
                            showMainWindow = true;
                            SystemSounds.Beep.Play();
                            log.Info($"Last result for {task.TaskName} was {task.TaskResult}, showing {AppInfo.AppName} window");
                            break;
                        }
                    }

                    // If showMainWindow is false, then shut down
                    if (!showMainWindow)
                    {
                        log.Info("No scheduled tasks with a non-zero results were found.");
                        Application.Current.Shutdown();
                    }
                }
                else
                {
                    if (item != args[0])
                    {
                        log.Warn($"Unknown command line argument  \"{item}\" found.");
                    }
                }
            }
        }
        #endregion Command line arguments

        #region Write the tasks JSON file
        private void WriteTasks2Json(bool suppress)
        {
            // Convert MyTasksCollection to JSON and save it to a file
            try
            {
                string x = JsonConvert.SerializeObject(MyTasks.MyTasksCollection, Formatting.Indented);
                File.WriteAllText(tasksFile, x);
                log.Info($"Writing {MyTasks.MyTasksCollection.Count} items to {tasksFile} ");

                if (!suppress)
                {
                    _ = TKMessageBox.Show($"Saved to {tasksFile}",
                                "File Saved",
                                MessageBoxButton.OK,
                                MessageBoxImage.FileSave);
                }
                MyTasks.IsDirty = false;
                sbRight.Text = string.Empty;
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error saving {tasksFile}");
                _ = TKMessageBox.Show($"Error saving {tasksFile}\n\n{ex.Message}",
                                    "Error!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
        }
        #endregion Write the tasks JSON file

        #region Set datagrid column sort
        public void SetDGColumnSort()
        {
            if (File.Exists(colsFile))
            {
                DataGridTasks.Items.SortDescriptions.Clear();
                string json = File.ReadAllText(colsFile);
                var q = JsonConvert.DeserializeObject<List<ColumnSort>>(json);
                foreach (var item in q)
                {
                    DataGridColumn column = DataGridTasks.Columns[item.DisplayIndex];
                    if (item.SortDirection == "Ascending")
                    {
                        DataGridTasks.Items.SortDescriptions.Add(new SortDescription(
                            column.SortMemberPath, ListSortDirection.Ascending));
                        column.SortDirection = ListSortDirection.Ascending;
                        log.Info($"Column \"{column.Header}\" sort is Ascending");
                    }
                    else if (item.SortDirection == "Descending")
                    {
                        DataGridTasks.Items.SortDescriptions.Add(new SortDescription(
                            column.SortMemberPath, ListSortDirection.Descending));
                        column.SortDirection = ListSortDirection.Descending;
                        log.Info($"Column \"{column.Header}\" sort is Descending");
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

        #region Save datagrid columns sort
        public void SaveDGColumnSort()
        {
            List<ColumnSort> csList = new List<ColumnSort>();
            foreach (DataGridColumn col in DataGridTasks.Columns)
            {
                if (col.SortDirection != null)
                {
                    log.Debug($"Saving column \"{col.Header}\" sort ({col.SortDirection})");
                    csList.Add(new ColumnSort
                    {
                        Header = col.Header.ToString(),
                        DisplayIndex = col.DisplayIndex,
                        SortDirection = col.SortDirection.ToString()
                    });
                }
                else
                {
                    csList.Add(new ColumnSort
                    {
                        Header = col.Header.ToString(),
                        DisplayIndex = col.DisplayIndex,
                        SortDirection = "null"
                    });
                }
            }

            try
            {
                string json = JsonConvert.SerializeObject(csList, Formatting.Indented);
                File.WriteAllText(colsFile, json);
            }
            catch (Exception ex)
            {
                log.Error(ex, $"Error saving {colsFile}");
                _ = TKMessageBox.Show($"Error saving {colsFile}\n\n{ex.Message}",
                                    "Error!",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
            }
        }
        #endregion Save datagrid columns sort

        #region Get scheduled task info for one task
        public static Task GetTaskInfo(string name)
        {
            using (TaskService ts = new TaskService())
            {
                Task task = ts.GetTask(name);

                return task;
            }
        }

        #endregion Get scheduled task info for one task

        #region Get updated tasks from TaskList
        private void UpdateMyTasksCollection()
        {
            log.Debug($"MyTasksCollection before update: {MyTasks.MyTasksCollection.Count}");
            MyTasks.MyTasksCollection.Clear();
            foreach (ScheduledTask item in ScheduledTask.TaskList)
            {
                if (item.TaskPath != null)
                {
                    MyTasks.MyTasksCollection.Add(new MyTasks(item.TaskPath, item.IsChecked, item.TaskNote));
                }
            }
            log.Debug($"MyTasksCollection after update: {MyTasks.MyTasksCollection.Count}");
        }
        #endregion Get updated tasks from TaskList

        #region Refresh
        private void RefreshData()
        {
            if (MyTasks.IsDirty)
            {
                UpdateMyTasksCollection();
                WriteTasks2Json(true);
                ReadMyTasks();
            }
            ScheduledTask.TaskList.Clear();
            LoadData();
            DataGridTasks.ItemsSource = ScheduledTask.TaskList;

            // MyTasks.MyTasksCollection isn't really dirty, it's been reloaded with the same data
            // so we reset the dirty flag and turn off the warning message in the status bar.
            MyTasks.IsDirty = false;
            sbRight.Text = string.Empty;
        }
        #endregion

        #region Show Add window
        private void ShowAddWindow()
        {
            //AddTaskWindow addWindow = new AddTaskWindow()
            AddSelectWindow addWindow = new AddSelectWindow()
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            addWindow.ShowDialog();
        }
        #endregion Show Add window

        #region Window events
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            SetDGColumnSort();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (MyTasks.IsDirty)
            {
                if (Settings.Default.SaveOnExit)
                {
                    UpdateMyTasksCollection();
                    WriteTasks2Json(Settings.Default.SuppressFileSaveNotify);
                }
                else
                {
                    var result = TKMessageBox.Show("Save changes before exiting? \n\n" +
                               "Click YES to save changes and exit.\n" +
                               "Click NO to exit without saving.\n" +
                               "Click CANCEL to go back.",
                    "Confirm exit",
                               MessageBoxButton.YesNoCancel,
                               MessageBoxImage.Question);

                    if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else if (result == MessageBoxResult.Yes)
                    {
                        UpdateMyTasksCollection();
                        WriteTasks2Json(Settings.Default.SuppressFileSaveNotify);
                    }
                }
            }

            if (MyTasks.SortIsDirty)
            {
                SaveDGColumnSort();
            }

            stopwatch.Stop();
            string line = string.Format("{0} is shutting down.  {1:g} elapsed time.", AppInfo.AppName, stopwatch.Elapsed);
            log.Info(line);

            LogManager.Shutdown();

            // save the property settings
            Settings.Default.WindowLeft = Left;
            Settings.Default.WindowTop = Top;
            Settings.Default.Save();
        }
        #endregion Window events

        #region Datagrid events
        private void DataGridTasks_Sorting(object sender, DataGridSortingEventArgs e)
        {
            log.Debug($"DataGrid sorting event: {e.Column.DisplayIndex} {e.Column.SortDirection} {e.Column.SortMemberPath}");

            MyTasks.SortIsDirty = true;
        }
        #endregion Datagrid events

        #region Mouse Events
        private void DataGridTasks_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
                return;

            if (e.Delta > 0)
            {
                GridLarger();
            }
            else if (e.Delta < 0)
            {
                GridSmaller();
            }
        }
        #endregion Mouse Events

        #region Menu events
        private void MnuExit_Click(object sender, RoutedEventArgs e)
        {
            if (MyTasks.IsDirty)
            {
                if (Settings.Default.SaveOnExit)
                {
                    UpdateMyTasksCollection();
                    WriteTasks2Json(Settings.Default.SuppressFileSaveNotify);
                }
                else
                {
                    var result = TKMessageBox.Show("Save changes before exiting? \n\n" +
                               "Click YES to save changes and exit.\n" +
                               "Click NO to exit without saving.\n" +
                               "Click CANCEL to go back.",
                               "Confirm exit",
                               MessageBoxButton.YesNoCancel,
                               MessageBoxImage.Question);

                    if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
                    {
                        return;
                    }
                    else if (result == MessageBoxResult.Yes)
                    {
                        UpdateMyTasksCollection();
                        WriteTasks2Json(Settings.Default.SuppressFileSaveNotify);
                    }
                }
            }
            Application.Current.Shutdown();
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult reply = TKMessageBox.Show("Restart with Elevated Permissions?",
                                                       "Restart?",
                                                        MessageBoxButton.OKCancel,
                                                        MessageBoxImage.Question);

            if (reply == MessageBoxResult.OK)
            {
                log.Info($"{AppInfo.AppName} is restarting with Elevated Permissions");
                using (Process p = new Process())
                {
                    p.StartInfo.FileName = Application.ResourceAssembly.Location;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.Verb = "runas";
                    p.Start();

                    Application.Current.Shutdown();
                }
            }
        }

        private void Collapse_Click(object sender, RoutedEventArgs e)
        {
            IEnumerable<DataGridRow> rows = GetDataGridRows(DataGridTasks);
            foreach (DataGridRow row in rows)
            {
                row.DetailsVisibility = Visibility.Collapsed;
            }
        }

        private void MnuViewReadMe_Click(object sender, RoutedEventArgs e)
        {
            TextFileViewer.ViewTextFile(Path.Combine(AppInfo.AppDirectory, "ReadMe.txt"));
        }

        private void MnuAbout_Click(object sender, RoutedEventArgs e)
        {
            About about = new About
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            _ = about.ShowDialog();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            ShowAddWindow();
        }

        private void GridSmaller_Click(object sender, RoutedEventArgs e)
        {
            GridSmaller();
        }

        private void GridLarger_Click(object sender, RoutedEventArgs e)
        {
            GridLarger();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            UpdateMyTasksCollection();
            WriteTasks2Json(false);
        }

        private void Tasks_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            mnuDelete.IsEnabled = DataGridTasks.SelectedIndex != -1;
        }

        private void ResetCols_Click(object sender, RoutedEventArgs e)
        {
            foreach (var column in DataGridTasks.Columns)
            {
                column.SortDirection = null;
            }
            DataGridTasks.Items.SortDescriptions.Clear();
            SaveDGColumnSort();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridTasks.SelectedItems.Count > 0)
            {
                for (int i = DataGridTasks.SelectedItems.Count - 1; i >= 0; i--)
                {
                    var row = (ScheduledTask)DataGridTasks.SelectedItems[i];
                    ScheduledTask.TaskList.Remove(row);
                    log.Info($"Removed \"{row.TaskPath}\"");
                }
            }
        }

        private void OpenTaskSched_Click(object sender, RoutedEventArgs e)
        {
            using (Process taskSched = new Process())
            {
                taskSched.StartInfo.FileName = "mmc.exe";
                taskSched.StartInfo.Arguments = @"c:\windows\system32\taskschd.msc";
                taskSched.Start();
            }
        }

        private void ViewJson_Click(object sender, RoutedEventArgs e)
        {
            TextFileViewer.ViewTextFile(tasksFile);
        }

        private void ViewTemp_Click(object sender, RoutedEventArgs e)
        {
            TextFileViewer.ViewTextFile(GetTempFile());
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshData();
        }

        private void Copy2Clipboard_Click(object sender, RoutedEventArgs e)
        {
            string fullName = AppInfo.AppPath;
            Clipboard.SetText(fullName);
            log.Debug($"{fullName} copied to clipboard");
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

        private void EnableTask_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridTasks.SelectedItems.Count == 1)
            {
                var row = DataGridTasks.SelectedItem as ScheduledTask;
                string taskPath = row.TaskPath;
                EnableTask(taskPath);
            }
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (DataGridTasks.SelectedItems.Count != 1 || !IsAdministrator())
            {
                mnuDisable.IsEnabled = false;
                mnuEnable.IsEnabled = false;
                mnuRunTask.IsEnabled = false;
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

        #endregion Menu events

        #region Keyboard events
        private void Window_Keydown(object sender, KeyEventArgs e)
        {
            // F1 opens Help menu
            if (e.Key == Key.F1)
            {
                mnuHelp.IsSubmenuOpen = true;
            }

            // F5 = Refresh
            if (e.Key == Key.F5)
            {
                mnuRefresh.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            }

            // Ctrl + N = Add
            if (e.Key == Key.N && (e.KeyboardDevice.Modifiers == ModifierKeys.Control))
            {
                mnuAdd.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            }

            // Ctrl + D = Delete
            if (e.Key == Key.D && (e.KeyboardDevice.Modifiers == ModifierKeys.Control))
            {
                mnuDelete.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            }

            // Ctrl + S = Save
            if (e.Key == Key.S && (e.KeyboardDevice.Modifiers == ModifierKeys.Control))
            {
                mnuSave.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            }

            // Ctrl + comma opens Options menu
            if (e.Key == Key.OemComma && (e.KeyboardDevice.Modifiers == ModifierKeys.Control))
            {
                mnuOptions.IsSubmenuOpen = true;
            }
        }

        private void DataGridTasks_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Delete = Delete selected rows
            if (e.Key == Key.Delete)
            {
                mnuDelete.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
            }
        }
        #endregion Keyboard events

        #region Enumerate Datagrid rows
        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource;
            if (itemsSource == null)
            {
                yield return null;
            }
            else
            {
                foreach (var item in itemsSource)
                {
                    if (grid.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row)
                    {
                        yield return row;
                    }
                }
            }
        }
        #endregion Enumerate Datagrid rows

        #region List changes
        // The BindingList can notify when the checkbox changes
        private void Binding_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e != null)
            {
                log.Debug($"List Changed Type was: {e.ListChangedType}");
                MyTasks.IsDirty = true;
                sbRight.Text = "Unsaved changes";
            }
        }

        // The collection change will notify when a list item is added or deleted
        private void TaskList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            log.Debug($"Collection Changed Action was: {e.Action}");
            MyTasks.IsDirty = true;
            sbRight.Text = "Unsaved changes";
            sbLeft.Content = ScheduledTask.TaskList.Count;
        }
        #endregion List changes

        #region Setting change
        private void SettingChanging(object sender, SettingChangingEventArgs e)
        {
            switch (e.SettingName)
            {
                case "ShadeAltRows":
                    {
                        if ((bool)e.NewValue)
                        {
                            AltRowShadingOn();
                        }
                        else
                        {
                            AltRowShadingOff();
                        }
                        break;
                    }
            }
            log.Debug($"Setting: {e.SettingName} New Value: {e.NewValue}");
        }
        #endregion Setting change

        #region Datagrid row double-click handler
        private void RowDoubleClick(object sender, RoutedEventArgs e)
        {
            var row = (DataGridRow)sender;
            row.DetailsVisibility = row.DetailsVisibility == Visibility.Collapsed ?
                Visibility.Visible : Visibility.Collapsed;
        }
        #endregion Datagrid row double-click handler

        #region Grid Size
        private void GridSmaller()
        {
            double curZoom = Settings.Default.GridZoom;
            if (curZoom > 0.8)
            {
                curZoom -= .05;
                Settings.Default.GridZoom = curZoom;
            }

            DataGridTasks.LayoutTransform = new ScaleTransform(curZoom, curZoom);
        }

        private void GridLarger()
        {
            double curZoom = Settings.Default.GridZoom;
            if (curZoom < 1.5)
            {
                curZoom += .05;
                Settings.Default.GridZoom = curZoom;
            }

            DataGridTasks.LayoutTransform = new ScaleTransform(curZoom, curZoom);
        }
        #endregion Grid Size

        #region Alternate row shading
        private void AltRowShadingOff()
        {
            DataGridTasks.AlternationCount = 0;
            DataGridTasks.RowBackground = new SolidColorBrush(Colors.White);
            DataGridTasks.AlternatingRowBackground = new SolidColorBrush(Colors.White);
            DataGridTasks.Items.Refresh();
        }

        private void AltRowShadingOn()
        {
            DataGridTasks.AlternationCount = 2;
            DataGridTasks.RowBackground = new SolidColorBrush(Colors.White);
            DataGridTasks.AlternatingRowBackground = new SolidColorBrush(Colors.WhiteSmoke);
            DataGridTasks.Items.Refresh();
        }
        #endregion Alternate row shading

        #region Window Title
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

        public static bool IsAdministrator()
        {
            return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
        }
        #endregion Window Title

        #region Get temp file name
        public static string GetTempFile()
        {
            // Ask NLog what the file name is
            var target = LogManager.Configuration.FindTargetByName("logFile") as FileTarget;
            var logEventInfo = new LogEventInfo { TimeStamp = DateTime.Now };
            string fileName = target.FileName.Render(logEventInfo);
            return fileName;
        }
        #endregion

        #region Unhandled Exception Handler
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
        }
        #endregion Unhandled Exception Handler

        #region Disable, Enable and Run tasks
        private void DisableTask(string taskName)
        {
            using (TaskService ts = new TaskService())
            {
                Task task = ts.GetTask(taskName);
                if (task == null)
                    return;
                try
                {
                    task.Enabled = false;
                    RefreshData();
                    sbRight.Text = $"Disabled: {task.Name}";
                    log.Info("Disabled {0}", task.Path);
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    sbRight.Text = ex.Message;
                    log.Error(ex, "Error attempting to disable {0}", task.Name);
                }
            }
        }

        private void EnableTask(string taskName)
        {
            using (TaskService ts = new TaskService())
            {
                Task task = ts.GetTask(taskName);
                if (task == null)
                    return;
                try
                {
                    task.Enabled = true;
                    RefreshData();
                    sbRight.Text = $"Enabled: {task.Name}";
                    log.Info("Enabled {0}", task.Path);
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    sbRight.Text = ex.Message;
                    log.Error(ex, "Error attempting to enable {0}", task.Name);
                }
            }
        }

        private void RunTask(string taskName)
        {
            using (TaskService ts = new TaskService())
            {
                Task task = ts.GetTask(taskName);

                if (task == null)
                    return;

                try
                {
                    task.Run();
                    RefreshData();
                    sbRight.Text = $"Running: {task.Name}";
                    log.Info("Running {0}", task.Path);
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    sbRight.Text = ex.Message;
                    log.Error(ex, "Error attempting to run {0}", task.Name);
                }
            }
        }
        #endregion Disable, Enable and Run tasks
    }
}