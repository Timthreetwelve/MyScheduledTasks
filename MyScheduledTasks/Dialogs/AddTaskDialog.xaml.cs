// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Dialogs;

/// <summary>
/// Dialog that facilitates adding tasks
/// </summary>
public partial class AddTaskDialog : UserControl
{
    #region NLog Instance
    private static readonly Logger log = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    public AddTaskDialog()
    {
        InitializeComponent();

        ReadSettings();

        GetTaskList();
    }

    #region Read Settings
    private void ReadSettings()
    {
        // Set height and width
        Width = MainWindow.Instance.Width - 150;
        Height = MainWindow.Instance.ActualHeight - 150;
        MinWidth = 600;
        MinHeight = 300;

        // Hide Microsoft
        cbxHideMicroSoft.IsChecked = UserSettings.Setting.HideMicrosoftFolder;

        SetRowSpacing((Spacing)UserSettings.Setting.RowSpacing);

        // Settings change event
        UserSettings.Setting.PropertyChanged += UserSettingChanged;
    }
    #endregion Read Settings

    #region Setting change
    /// <summary>
    /// My way of handling changes in UserSettings
    /// </summary>
    private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
        object newValue = prop?.GetValue(sender, null);
        switch (e.PropertyName)
        {
            case nameof(UserSettings.Setting.RowSpacing):
                SetRowSpacing((Spacing)newValue);
                break;
        }
    }
    #endregion Setting change

    #region Get list of tasks
    private void GetTaskList()
    {
        List<string> taskList = new();

        using (TaskService ts = new())
        {
            foreach (Task task in ts.AllTasks)
            {
                if (cbxHideMicroSoft.IsChecked == true)
                {
                    if (!task.Path.StartsWith(@"\Microsoft"))
                    {
                        taskList.Add(task.Path);
                    }
                }
                else
                {
                    taskList.Add(task.Path);
                }
            }
        }
        listBox.ItemsSource = taskList;
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
                    _ = new MDCustMsgBox($"The Scheduled Task \"{item}\" was not found.", "ERROR", ButtonType.Ok).ShowDialog();
                    return;
                }
                else if (ScheduledTask.TaskList.Any(p => p.TaskPath == task.Path))
                {
                    log.Warn($"{task.Path} has already been added");
                    log.Debug($"{task.Path} is already in the list");
                    continue;
                }
                ScheduledTask schedTask = ScheduledTask.BuildSchedTask(task, null);
                ScheduledTask.TaskList.Add(schedTask);

                MyTasks my = new(task.Path, false, string.Empty);
                MyTasks.MyTasksCollection.Add(my);

                log.Info($"Added {task.Path}");
            }
            MainWindow.Instance.RefreshData();
            listBox.UnselectAll();
        }
    }
    #endregion Add selected items to TaskList

    #region Get info for a task
    public static Task GetTaskInfo(string name)
    {
        using TaskService ts = new();
        return ts.GetTask(name);
    }
    #endregion Get info for a task

    #region Checkbox events
    private void CbxHideMicroSoft_Checked(object sender, RoutedEventArgs e)
    {
        if (IsVisible)
        {
            //listBox.Items.Clear();
            GetTaskList();
            listBox.InvalidateArrange();
            listBox.UpdateLayout();
            FilterTheList();
            UserSettings.Setting.HideMicrosoftFolder = (bool)cbxHideMicroSoft.IsChecked;
        }
    }
    #endregion Checkbox events

    #region Button Events
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        AddSelectedItems();
    }
    #endregion Button Events

    #region Mouse wheel events
    private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            if (e.Delta > 0)
            {
                MainWindow.Instance.EverythingLarger();
            }
            else if (e.Delta < 0)
            {
                MainWindow.Instance.EverythingSmaller();
            }
        }
    }
    #endregion Mouse wheel events

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
                listBox.ItemContainerStyle = Application.Current.FindResource("ListBoxCompact") as Style;
                break;
            case Spacing.Comfortable:
                listBox.ItemContainerStyle = Application.Current.FindResource("ListBoxComfortable") as Style;
                break;
            case Spacing.Wide:
                listBox.ItemContainerStyle = Application.Current.FindResource("ListBoxSpacious") as Style;
                break;
        }
    }
    #endregion Set the row spacing

    #region Filter the list
    private void TbxSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        FilterTheList();
    }

    private void FilterTheList()
    {
        //string filter = tbxSearch.Text;

        if (string.IsNullOrEmpty(tbxSearch.Text))
        {
            CollectionViewSource.GetDefaultView(listBox.ItemsSource).Filter = null;
            return;
        }

        if (listBox.ItemsSource != null)
        {
            CollectionViewSource.GetDefaultView(listBox.ItemsSource).Filter = MyFilter;
        }
    }

    private bool MyFilter(object obj)
    {
        return obj.ToString().Contains(tbxSearch.Text, StringComparison.OrdinalIgnoreCase);
    }
    #endregion Filter the list
}
