// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class TaskHelpers
{
    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region List all tasks
    /// <summary>
    /// Gets all scheduled tasks.
    /// Creates a list for all tasks and one for tasks not in the Microsoft folder.
    /// </summary>
    /// <remarks>Some tasks may not be listed when not running as administrator.</remarks>
    internal static void GetAllTasks()
    {
        AllTasks.All_TasksCollection.Clear();
        AllTasks.Non_MS_TasksCollection.Clear();

        Stopwatch stopwatch = Stopwatch.StartNew();
        using TaskService ts = TaskService.Instance;
        foreach (Task task in ts.AllTasks)
        {
            AllTasks allTasks = new()
            {
                TaskPath = task.Path,
                TaskName = task.Name,
                TaskFolder = task.Folder.Path,
            };
            AllTasks.All_TasksCollection.Add(allTasks);
            if (!task.Folder.Path.StartsWith(@"\Microsoft"))
            {
                AllTasks.Non_MS_TasksCollection.Add(allTasks);
            }
        }
        stopwatch.Stop();
        _log.Debug($"GetAllTasks found {AllTasks.All_TasksCollection.Count}/{AllTasks.Non_MS_TasksCollection.Count} tasks took {stopwatch.Elapsed.TotalSeconds} seconds.");
    }
    #endregion List all tasks

    #region Remove tasks
    internal static void RemoveTasks(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            return;
        }

        if (grid.SelectedItems.Count <= 5)
        {
            for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
            {
                ScheduledTask row = grid.SelectedItems[i] as ScheduledTask;
                _ = ScheduledTask.TaskList.Remove(row);
                _log.Info($"Removed \"{row.TaskPath}\"");
                SnackbarMsg.QueueMessage($"Removed {row.TaskName}", 1000);
            }
        }
        else if (grid.SelectedItems.Count > 3)
        {
            int count = grid.SelectedItems.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                ScheduledTask row = grid.SelectedItems[i] as ScheduledTask;
                ScheduledTask.TaskList.Remove(row);
                _log.Info($"Removed \"{row.TaskPath}\"");
            }
            SnackbarMsg.QueueMessage($"Removed {count} tasks", 2000);
        }
    }
    #endregion Remove tasks

    #region Run tasks
    internal static void RunTask(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage("Nothing selected to run.", 5000);
            return;
        }

        for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
        {
            ScheduledTask row = grid.SelectedItems[i] as ScheduledTask;
            using TaskService ts = TaskService.Instance;
            Task task = ts.GetTask(row.TaskPath);

            if (task != null)
            {
                try
                {
                    task.Run();
                    SnackbarMsg.ClearAndQueueMessage($"Running: {task.Name}");
                    _log.Info($"Running {task.Path}");
                    System.Threading.Tasks.Task.Delay(1250).Wait();
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    SnackbarMsg.ClearAndQueueMessage($"Error attempting to run {task.Name}. See the log file for details.", 5000);
                    _log.Error(ex, $"Error attempting to run {task.Name}");
                }
            }
        }
    }
    #endregion Run a single task

    #region Disable Tasks
    internal static void DisableTask(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage("Nothing selected to disable.", 5000);
            return;
        }

        for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
        {
            ScheduledTask row = grid.SelectedItems[i] as ScheduledTask;
            using TaskService ts = TaskService.Instance;
            Task task = ts.GetTask(row.TaskPath);

            if (task != null)
            {
                try
                {
                    task.Enabled = false;
                    //RefreshData();
                    SnackbarMsg.QueueMessage($"Disabled: {task.Name}", 2000);
                    _log.Info($"Disabled {task.Path}");
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    SnackbarMsg.ClearAndQueueMessage($"Error attempting to disable {task.Name}", 5000);
                    _log.Error(ex, $"Error attempting to disable {task.Name}");
                }
            }
        }
    }
    #endregion Disable Tasks

    #region Enable Tasks
    internal static void EnableTask(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage("Nothing selected to enable.", 5000);
            return;
        }

        for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
        {
            ScheduledTask row = grid.SelectedItems[i] as ScheduledTask;
            using TaskService ts = TaskService.Instance;
            Task task = ts.GetTask(row.TaskPath);

            if (task != null)
            {
                try
                {
                    task.Enabled = true;
                    //RefreshData();
                    SnackbarMsg.QueueMessage($"Enabled: {task.Name}", 2000);
                    _log.Info($"Enabled {task.Path}");
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    SnackbarMsg.ClearAndQueueMessage($"Error attempting to enable {task.Name}", 5000);
                    _log.Error(ex, $"Error attempting to enable {task.Name}");
                }
            }
        }
    }
    #endregion Enable Tasks

    #region Export Tasks
    internal static void ExportTask(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage("Nothing selected to export.", 5000);
            return;
        }

        for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
        {
            ScheduledTask row = grid.SelectedItems[i] as ScheduledTask;
            using TaskService ts = TaskService.Instance;
            Task task = ts.GetTask(row.TaskPath);

            if (task != null)
            {
                try
                {
                    SaveFileDialog dialog = new()
                    {
                        Title = "Export Task to XML File",
                        Filter = "XML File|*.xml",
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                        FileName = task.Name + ".xml"
                    };
                    bool? result = dialog.ShowDialog();
                    if (result == true)
                    {
                        task.Export(dialog.FileName);
                        SnackbarMsg.ClearAndQueueMessage($"Exported: {task.Name}");
                        _log.Info($"Exported {task.Path}");
                    }
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    SnackbarMsg.ClearAndQueueMessage($"Error attempting to export {task.Name}", 5000);
                    _log.Error(ex, $"Error attempting to export {task.Path}");
                }
            }
        }
    }
    #endregion

    #region Import a task
    internal static void ImportTasks()
    {
        try
        {
            TaskDefinition td = TaskService.Instance.NewTaskFromFile(ImportTask.ImportTaskXML);
            td.Principal.LogonType = TaskLogonType.InteractiveToken;
            td.Principal.RunLevel = TaskRunLevel.Highest;
            _ = TaskService.Instance.RootFolder.RegisterTaskDefinition(ImportTask.ImportTaskName, td);
            SnackbarMsg.ClearAndQueueMessage($"Imported: {ImportTask.ImportTaskXML}");
            _log.Info($"Imported {ImportTask.ImportTaskXML} to {ImportTask.ImportTaskName}");
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error Importing {ImportTask.ImportTaskXML}");
            MDCustMsgBox mbox = new($"Error importing task.\n\n{ex.Message}",
                    "Replace this with localized text",
                    ButtonType.Ok,
                    false,
                    true,
                    _mainWindow,
                    true);
            _ = mbox.ShowDialog();

        }
    }
    #endregion Import a task

    public static bool CheckTaskExists(string taskPath)
    {
        TaskService ts = TaskService.Instance;
        Task task = ts.GetTask(taskPath);
        ts.Dispose();
        return task != null;
    }
}
