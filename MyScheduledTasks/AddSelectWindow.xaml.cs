// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using Microsoft.Win32.TaskScheduler;
using NLog;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using TKUtils;
using MessageBoxImage = TKUtils.MessageBoxImage;
#endregion

namespace MyScheduledTasks
{
    public partial class AddSelectWindow : Window
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        #region Constructor
        public AddSelectWindow()
        {
            InitializeComponent();

            ReadSettings();

            GetTaskList();
        }
        #endregion

        #region Read Settings
        private void ReadSettings()
        {
            // Set screen metrics
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 20;
            MaxWidth = SystemParameters.PrimaryScreenWidth - 20;
            Height = Properties.Settings.Default.AddWindowHeight;
            Width = Properties.Settings.Default.AddWindowWidth;

            // Set listbox zoom
            double curZoom = Properties.Settings.Default.GridZoom;
            listBox.LayoutTransform = new ScaleTransform(curZoom, curZoom);
        }
        #endregion

        #region Get list of tasks
        private void GetTaskList()
        {
            using (TaskService ts = new TaskService())
            {
                foreach (Task task in ts.AllTasks)
                {
                    _ = listBox.Items.Add(task.Path);
                }
            }
        }
        #endregion

        #region Get info for a task
        public static Task GetTaskInfo(string name)
        {
            using (TaskService ts = new TaskService())
            {
                Task task = ts.GetTask(name);
                return task;
            }
        }
        #endregion

        #region Add selected items to TaskList
        private void AddSelectedItems()
        {
            if (listBox.SelectedItems.Count > 0)
            {
                foreach (var item in listBox.SelectedItems)
                {
                    Task task = GetTaskInfo(item.ToString());

                    if (task == null)
                    {
                        log.Error($"The Scheduled Task \"{item}\" was not found.");
                        _ = TKMessageBox.Show($"The Scheduled Task \"{item}\" was not found.",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }
                    else if (ScheduledTask.TaskList.Any(p => p.TaskPath == task.Path))
                    {
                        log.Warn($"{task.Path} has already been added");
                        log.Debug($"{task.Path} is already in the list");
                        continue;
                    }

                    // *****************************************************
                    // if changes are made to the following, change the
                    // corresponding code in the MainWindow.LoadData method.
                    // *****************************************************
                    string folder = task.Path.Replace(task.Name, "");
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
                        IsChecked = false
                    };

                    ScheduledTask.TaskList.Add(schedTask);

                    MyTasks my = new MyTasks(task.Path, false);
                    MyTasks.MyTasksCollection.Add(my);

                    log.Info($"Added {task.Path}");
                }
                listBox.UnselectAll();
            }
        }
        #endregion

        #region Button & window events
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddSelectedItems();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.AddWindowHeight = Height;
            Properties.Settings.Default.AddWindowWidth = Width;
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}
