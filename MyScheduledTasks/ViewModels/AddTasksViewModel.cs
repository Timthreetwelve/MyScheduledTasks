﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.ViewModels;

internal sealed partial class AddTasksViewModel
{
    #region Private field
    private static int _itemsAdded;
    #endregion Private field

    #region Add selected items to TaskList
    /// <summary>
    /// Add all selected items
    /// </summary>
    /// <param name="grid">Name of the DataGrid</param>
    private static void AddSelectedItems(DataGrid grid)
    {
        if (grid.SelectedItems.Count > 0)
        {
            _itemsAdded = 0;
            foreach (AllTasks item in grid.SelectedItems)
            {
                AddToMyTasks(item);
            }
            if (_itemsAdded > 0)
            {
                _log.Info($"{_itemsAdded} task(s) added");
                string msg = string.Format(CultureInfo.CurrentCulture, AddTasksTasksAdded, _itemsAdded);
                SnackbarMsg.QueueMessage(msg, 3000);
                TaskFileHelpers.WriteTasksToFile();
            }

            grid.UnselectAll();
        }
    }
    #endregion Add selected items to TaskList

    #region Add a single item
    /// <summary>
    /// Adds a single item to the tasks list
    /// </summary>
    /// <param name="item">Path of the item to be added</param>
    /// <returns>True if the task was added. Otherwise returns false.</returns>
    internal static bool AddToMyTasks(AllTasks item)
    {
        Task? task = GetTaskInfo(item.TaskPath!);
        if (task == null)
        {
            string msg = string.Format(CultureInfo.InvariantCulture, AddTasksNotFound, item.TaskName);
            _log.Error($"The Scheduled Task \"{item.TaskPath}\" was not found.");
            _ = new MDCustMsgBox(msg,
                    GetStringResource("AddTasks_Error"),
                    ButtonType.Ok,
                    false,
                    true,
                    null,
                    true).ShowDialog();
            return false;
        }

        if (ScheduledTask.TaskList.Any(p => p.TaskPath == task.Path))
        {
            int pos = ScheduledTask.TaskList.IndexOf(ScheduledTask.TaskList.FirstOrDefault(x => x.TaskPath == task.Path)!);
            _log.Warn($"{task.Path} is already present in the list in position {pos + 1}");
            string msg = string.Format(CultureInfo.InvariantCulture, AddTasksTaskAlreadyAdded, task.Path);
            SnackbarMsg.QueueMessage(msg, 3000);
            return false;
        }
        ScheduledTask schedTask = ScheduledTask.BuildScheduledTask(task, null);
        ScheduledTask.TaskList.Add(schedTask);

        MyTasks newTask = new(task.Path, false, string.Empty);
        MyTasks.MyTasksCollection!.Add(newTask);

        _log.Info($"Added: \"{task.Path}\"");
        _itemsAdded++;
        return true;
    }
    #endregion Add a single item

    #region Include or exclude Microsoft tasks
    /// <summary>
    /// Determine source of add tasks list
    /// </summary>
    /// <param name="grid">Name of the DataGrid</param>
    private static void DetermineSource(DataGrid grid)
    {
        grid.ItemsSource = UserSettings.Setting!.HideMicrosoftFolder ? AllTasks.Non_MS_TasksCollection : AllTasks.All_TasksCollection;
    }
    #endregion Include or exclude Microsoft tasks

    #region Get info for a task

    private static Task? GetTaskInfo(string name)
    {
        using TaskService ts = new();
        return ts.GetTask(name);
    }
    #endregion Get info for a task

    #region Relay commands
    [RelayCommand]
    public static void HideTasks(DataGrid grid)
    {
        DetermineSource(grid);
    }

    [RelayCommand]
    private static void RefreshTasks()
    {
        TaskHelpers.GetAllTasks();
    }

    [RelayCommand]
    private static void AddTasks(DataGrid grid)
    {
        AddSelectedItems(grid);
    }
    #endregion Relay commands

    #region Static property and change event handler for filter text
    public static event EventHandler<PropertyChangedEventArgs>? StaticPropertyChanged;

    private static string? _filterText;
    public static string FilterText
    {
        get => _filterText!;
        set
        {
            if (_filterText != value)
            {
                _filterText = value;
                StaticPropertyChanged?.Invoke(null, new PropertyChangedEventArgs(FilterText));
            }
        }
    }
    #endregion Static property and change event handler for filter text
}
