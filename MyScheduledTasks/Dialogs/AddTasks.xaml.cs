using System.Linq;
using Task = Microsoft.Win32.TaskScheduler.Task;

namespace MyScheduledTasks.Dialogs
{
    /// <summary>
    /// Interaction logic for AddTasks.xaml
    /// </summary>
    public partial class AddTasks : UserControl
    {
        #region NLog Instance
        private static readonly Logger log = LogManager.GetLogger("logTemp");
        #endregion NLog Instance

        public AddTasks()
        {
            InitializeComponent();

            ReadSettings();

            GetTaskList();
        }

        #region Read Settings
        private void ReadSettings()
        {
            // Set screen metrics
            MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 20;
            MaxWidth = SystemParameters.PrimaryScreenWidth - 20;
            Height = UserSettings.Setting.AddWindowHeight;
            Width = UserSettings.Setting.AddWindowWidth;

            // Hide Microsoft
            cbxHideMicroSoft.IsChecked = UserSettings.Setting.HideMicrosoftFolder;
        }
        #endregion Read Settings

        #region Get list of tasks
        private void GetTaskList()
        {
            using (TaskService ts = new TaskService())
            {
                foreach (Task task in ts.AllTasks)
                {
                    if (cbxHideMicroSoft.IsChecked == true)
                    {
                        if (!task.Path.StartsWith(@"\Microsoft"))
                        {
                            _ = listBox.Items.Add(task.Path);
                        }
                    }
                    else
                    {
                        _ = listBox.Items.Add(task.Path);
                    }
                }
            }
            tbCounter.Text = $"{listBox.Items.Count} Tasks";
        }
        #endregion Get list of tasks

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
                        //_ = TKMessageBox.Show($"The Scheduled Task \"{item}\" was not found.",
                        //    "Error",
                        //    MessageBoxButton.OK,
                        //    MessageBoxImage.Error);
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
                        IsChecked = false
                    };

                    ScheduledTask.TaskList.Add(schedTask);

                    MyTasks my = new MyTasks(task.Path, false, string.Empty);
                    MyTasks.MyTasksCollection.Add(my);

                    log.Info($"Added {task.Path}");
                }
                listBox.UnselectAll();
            }
        }
        #endregion Add selected items to TaskList

        #region Get info for a task
        public static Task GetTaskInfo(string name)
        {
            using (TaskService ts = new TaskService())
            {
                return ts.GetTask(name);
            }
        }
        #endregion Get info for a task


        private void CbxHideMicroSoft_Checked(object sender, RoutedEventArgs e)
        {
            if (IsVisible)
            {
                listBox.Items.Clear();
                GetTaskList();
                listBox.InvalidateArrange();
                listBox.UpdateLayout();
                UserSettings.Setting.HideMicrosoftFolder = (bool)cbxHideMicroSoft.IsChecked;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddSelectedItems();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.Close(this);
        }
    }
}
