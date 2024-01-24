// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Dialogs;

/// <summary>
/// Dialog that facilitates adding tasks
/// </summary>
public partial class AddTaskDialog : UserControl
{
    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

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
        Width = _mainWindow.Width - 150;
        Height = _mainWindow.ActualHeight - 150;
        MinWidth = 600;
        MinHeight = 300;

        // Hide Microsoft
        //cbxHideMicroSoft.IsChecked = UserSettings.Setting.HideMicrosoftFolder;

        SetRowSpacing(UserSettings.Setting.RowSpacing);

        // Settings change event
        //UserSettings.Setting.PropertyChanged += UserSettingChanged;
    }
    #endregion Read Settings

    #region Setting change
    /// <summary>
    /// My way of handling changes in UserSettings
    /// </summary>
    private void UserSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        //PropertyInfo prop = sender.GetType().GetProperty(e.PropertyName);
        //object newValue = prop?.GetValue(sender, null);
        //switch (e.PropertyName)
        //{
        //    case nameof(UserSettings.Setting.RowSpacing):
        //        SetRowSpacing((Spacing)newValue);
        //        break;
        //}
    }
    #endregion Setting change

    #region Get list of tasks
    private void GetTaskList()
    {
        using (TaskService ts = new())
        {
            AllTasks.AllTasksCollection.Clear();

            foreach (Task task in ts.AllTasks)
            {
                AllTasks allTasks = new();

                if (cbxHideMicroSoft.IsChecked == true)
                {
                    if (!task.Path.StartsWith(@"\Microsoft"))
                    {
                        //taskList.Add(task.Path);
                        allTasks.TaskPath = task.Path;
                        AllTasks.AllTasksCollection.Add(allTasks);
                    }
                }
                else
                {
                    allTasks.TaskPath = task.Path;
                    AllTasks.AllTasksCollection.Add(allTasks);
                }
            }
        }
        //listBox.ItemsSource = taskList;
        tbCounter.Text = $"{listBox.Items.Count} Tasks";
    }
    #endregion Get list of tasks

    #region Add selected items to TaskList
    private void AddSelectedItems()
    {
        if (listBox.SelectedItems.Count > 0)
        {
            int itemsAdded = 0;
            foreach (AllTasks item in listBox.SelectedItems)
            {
                Task task = GetTaskInfo(item.TaskPath);
                if (task == null)
                {
                    _log.Error($"The Scheduled Task \"{item}\" was not found.");
                    _ = new MDCustMsgBox($"The Scheduled Task \"{item}\" was not found.", "ERROR", ButtonType.Ok).ShowDialog();
                    return;
                }
                else if (ScheduledTask.TaskList.Any(p => p.TaskPath == task.Path))
                {
                    int pos = ScheduledTask.TaskList.IndexOf(ScheduledTask.TaskList.FirstOrDefault(x => x.TaskPath == task.Path));
                    _log.Warn($"{task.Path} is already present in the list in position {pos + 1}");
                    continue;
                }
                ScheduledTask schedTask = ScheduledTask.BuildScheduledTask(task, null);
                ScheduledTask.TaskList.Add(schedTask);

                MyTasks my = new(task.Path, false, string.Empty);
                MyTasks.MyTasksCollection.Add(my);

                itemsAdded++;
                _log.Info($"Added {task.Path}");
            }
            if (itemsAdded > 0)
            {
                //MainWindow.Instance.RefreshData();
                _log.Info($"{itemsAdded} task(s) added");
            }

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
            //UserSettings.Setting.HideMicrosoftFolder = (bool)cbxHideMicroSoft.IsChecked;
        }
    }
    #endregion Checkbox events

    #region Button Events
    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        AddSelectedItems();
    }
    #endregion Button Events

    #region Set the row spacing
    /// <summary>
    /// Sets the padding around the rows in the ListBox
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
