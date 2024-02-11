// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.ViewModels;

internal partial class AddTasksViewModel
{
    internal static void UpdateItems(DataGrid grid)
    {
        if (UserSettings.Setting.HideMicrosoftFolder)
        {
            grid.ItemsSource = AllTasks.Non_MS_TasksCollection;
        }
        else
        {
            grid.ItemsSource = AllTasks.All_TasksCollection;
        }
    }

    #region Add selected items to TaskList
    private static void AddSelectedItems(DataGrid grid)
    {
        if (grid.SelectedItems.Count > 0)
        {
            int itemsAdded = 0;
            foreach (AllTasks item in grid.SelectedItems)
            {
                Task task = GetTaskInfo(item.TaskPath);
                if (task == null)
                {
                    _log.Error($"The Scheduled Task \"{item.TaskPath}\" was not found.");
                    _ = new MDCustMsgBox($"The Scheduled Task \"{item.TaskName}\" was not found.", "ERROR", ButtonType.Ok).ShowDialog();
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

                MyTasks newTask = new(task.Path, false, string.Empty);
                MyTasks.MyTasksCollection.Add(newTask);

                itemsAdded++;
                _log.Info($"Added {task.Path}");
            }
            if (itemsAdded > 0)
            {
                _log.Info($"{itemsAdded} task(s) added");
                SnackbarMsg.QueueMessage($"{itemsAdded} task(s) added",3000);
            }

            grid.UnselectAll();
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

    [RelayCommand]
    public static void HideTasks(DataGrid grid)
    {
        UpdateItems(grid);
    }

    [RelayCommand]
    public static void RefreshTasks()
    {
        TaskHelpers.GetAllTasks();
    }

    [RelayCommand]
    public static void AddTasks(DataGrid grid)
    {
        AddSelectedItems(grid);
    }

    public static event EventHandler<PropertyChangedEventArgs> StaticPropertyChanged;

    private static string _filterText;
    public static string FilterText
    {
        get => _filterText;
        set
        {
            if (_filterText != value)
            {
                _filterText = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(FilterText));
            }
        }
    }
}
