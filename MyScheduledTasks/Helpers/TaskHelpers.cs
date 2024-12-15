// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class TaskHelpers
{
    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Get all tasks from Windows Task Scheduler
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
        _log.Debug($"GetAllTasks found {AllTasks.All_TasksCollection.Count}/{AllTasks.Non_MS_TasksCollection.Count} tasks and took {stopwatch.Elapsed.TotalSeconds} seconds.");
    }
    #endregion Get all tasks from Windows Task Scheduler

    #region List tasks
    /// <summary>
    /// List tasks from TaskList, MyTasks, and in the DataGrid. Used for diagnostics.
    /// </summary>
    /// <param name="grid">The name of DataGrid</param>
    internal static void ListMyTasks(DataGrid grid)
    {
        _log.Debug("---------------------------------------------------------------");
        foreach (ScheduledTask item in ScheduledTask.TaskList)
        {
            _log.Debug($"TaskList: {item.TaskPath} {item.IsChecked} {item.TaskNote}");
        }
        _log.Debug("---------------------------------------------------------------");
        foreach (MyTasks item in MyTasks.MyTasksCollection!)
        {
            _log.Debug($"MyTasks: {item.TaskPath} {item.Alert} {item.TaskNote}");
        }
        _log.Debug("---------------------------------------------------------------");
        foreach (ScheduledTask row in grid.Items)
        {
            _log.Debug($"DataGrid: {row.TaskPath} {row.IsChecked} {row.TaskNote}");
        }
        _log.Debug("---------------------------------------------------------------");
    }
    #endregion List tasks

    #region Remove tasks
    /// <summary>
    /// Remove tasks from the grid and MyTasks
    /// </summary>
    /// <param name="grid">The name of the DataGrid</param>
    internal static void RemoveTasks(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            return;
        }

        if (grid.SelectedItems.Count <= 3)
        {
            for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
            {
                ScheduledTask? task = grid.SelectedItems[i] as ScheduledTask;
                _ = ScheduledTask.TaskList.Remove(task!);
                _log.Info($"Removed: \"{task!.TaskName}\"");
                SnackbarMsg.QueueMessage($"{GetStringResource("MsgText_Removed")} {task.TaskName}", 2000);
            }
        }
        else if (grid.SelectedItems.Count > 3)
        {
            int count = grid.SelectedItems.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                ScheduledTask? task = grid.SelectedItems[i] as ScheduledTask;
                _ = ScheduledTask.TaskList.Remove(task!);
                _log.Info($"Removed: \"{task!.TaskPath}\"");
            }
            SnackbarMsg.QueueMessage($"{GetStringResource("MsgText_Removed")} {count} {GetStringResource("MsgText_Tasks")}", 2000);
        }

        UpdateMyTasksCollection();
        TaskFileHelpers.WriteTasksToFile(true);
    }
    #endregion Remove tasks

    #region Get updated tasks from TaskList
    /// <summary>
    /// Update MyTasks from TaskList
    /// </summary>
    private static void UpdateMyTasksCollection()
    {
        MyTasks.MyTasksCollection!.Clear();
        for (int i = 0; i < ScheduledTask.TaskList.Count; i++)
        {
            ScheduledTask item = ScheduledTask.TaskList[i];
            if (item.TaskPath is not null)
            {
                MyTasks.MyTasksCollection.Add(new MyTasks(item.TaskPath, item.IsChecked, item.TaskNote));
            }
        }
    }
    #endregion Get updated tasks from TaskList

    #region Run tasks
    /// <summary>
    /// Execute the selected tasks
    /// </summary>
    /// <param name="grid">Name of the DataGrid</param>
    internal static void RunTask(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_RunNoneSelected"), 5000);
            return;
        }

        for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
        {
            ScheduledTask? row = grid.SelectedItems[i] as ScheduledTask;
            using TaskService ts = TaskService.Instance;
            using Task? task = ts.GetTask(row!.TaskPath);

            if (task != null)
            {
                try
                {
                    _log.Info($"Running: \"{task.Path}\"");
                    _ = task.Run();
                    SnackbarMsg.QueueMessage($"{GetStringResource("MsgText_Running")}: {task.Name}", 2000);
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    string msg = string.Format(GetStringResource("MsgText_RunError"), task.Name);
                    SnackbarMsg.ClearAndQueueMessage($"{msg} {GetStringResource("MsgText_SeeLogFile")}", 5000);
                    _log.Error(ex, $"Error attempting to run {task.Name}");
                }
            }
        }
    }
    #endregion Run a single task

    #region Disable Tasks
    /// <summary>
    /// Disable the selected tasks
    /// </summary>
    /// <param name="grid">Name of the DataGrid</param>
    internal static void DisableTask(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_DisableNoneSelected"), 5000);
            return;
        }

        for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
        {
            ScheduledTask? row = grid.SelectedItems[i] as ScheduledTask;
            using TaskService? ts = TaskService.Instance;
            using Task? task = ts.GetTask(row!.TaskPath);

            if (task != null)
            {
                try
                {
                    task.Enabled = false;
                    string msg = string.Format(GetStringResource("MsgText_Disabled"), task.Name);
                    SnackbarMsg.QueueMessage(msg, 2000);
                    _log.Info($"Disabled: \"{task.Path}\"");
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    string msg = string.Format(GetStringResource("MsgText_DisabledError"), task.Name);
                    SnackbarMsg.ClearAndQueueMessage($"{msg} {GetStringResource("MsgText_SeeLogFile")}", 5000);
                    _log.Error(ex, $"Error attempting to disable {task.Name}");
                }
            }
        }
    }
    #endregion Disable Tasks

    #region Enable Tasks
    /// <summary>
    /// Enable the selected tasks
    /// </summary>
    /// <param name="grid">Name of the DataGrid</param>
    internal static void EnableTask(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_EnableNoneSelected"), 5000);
            return;
        }

        for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
        {
            ScheduledTask? row = grid.SelectedItems[i] as ScheduledTask;
            using TaskService ts = TaskService.Instance;
            using Task? task = ts.GetTask(row!.TaskPath);

            if (task != null)
            {
                try
                {
                    task.Enabled = true;
                    string msg = string.Format(GetStringResource("MsgText_Enabled"), task.Name);
                    SnackbarMsg.QueueMessage(msg, 2000);
                    _log.Info($"Enabled: \"{task.Path}\"");
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    string msg = string.Format(GetStringResource("MsgText_EnableError"), task.Name);
                    SnackbarMsg.ClearAndQueueMessage($"{msg} {GetStringResource("MsgText_SeeLogFile")}", 5000);
                    _log.Error(ex, $"Error attempting to enable {task.Name}");
                }
            }
        }
    }
    #endregion Enable Tasks

    #region Export Tasks
    /// <summary>
    /// Export the selected tasks
    /// </summary>
    /// <param name="grid">Name of the DataGrid</param>
    internal static void ExportTask(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_ExportNoneSelected"), 5000);
            return;
        }

        for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
        {
            ScheduledTask? row = grid.SelectedItems[i] as ScheduledTask;
            using TaskService ts = TaskService.Instance;
            using Task? task = ts.GetTask(row!.TaskPath);

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
                        string msg = string.Format(GetStringResource("MsgText_Exported"), task.Name);
                        SnackbarMsg.QueueMessage(msg, 2000);
                        SnackbarMsg.ClearAndQueueMessage($"Exported: {task.Name}");
                        _log.Info($"Exported: \"{task.Path}\"");
                    }
                }
                catch (Exception ex)
                {
                    SystemSounds.Beep.Play();
                    string msg = string.Format(GetStringResource("MsgText_ExportError"), task.Name);
                    SnackbarMsg.ClearAndQueueMessage($"{msg} {GetStringResource("MsgText_SeeLogFile")}", 5000);
                    _log.Error(ex, $"Error attempting to export {task.Path}");
                }
            }
        }
    }
    #endregion

    #region Import a task
    /// <summary>
    /// Import a single task
    /// </summary>
    internal static void ImportTasks()
    {
        if (TempSettings.Setting!.ImportXMLFile!.Contains('\"'))
        {
            TempSettings.Setting.ImportXMLFile = TempSettings.Setting.ImportXMLFile.Trim('\"');
        }

        if (!File.Exists(TempSettings.Setting.ImportXMLFile))
        {
            MDCustMsgBox mbox = new($"{GetStringResource("ImportTask_FileNotFound")}\n\n{TempSettings.Setting.ImportXMLFile}",
                GetStringResource("ImportTask_ImportErrorHeader"),
                ButtonType.Ok,
                false,
                true,
                _mainWindow!,
                true);
            _ = mbox.ShowDialog();
            return;
        }

        if (string.IsNullOrEmpty(TempSettings.Setting.ImportTaskName))
        {
            MDCustMsgBox mbox = new(GetStringResource("ImportTask_ImportErrorBlank"),
                GetStringResource("ImportTask_ImportErrorHeader"),
                ButtonType.Ok,
                false,
                true,
                _mainWindow!,
                true);
            _ = mbox.ShowDialog();
            return;
        }

        if (!TempSettings.Setting.ImportTaskName.StartsWith('\\'))
        {
            TempSettings.Setting.ImportTaskName = TempSettings.Setting.ImportTaskName.Insert(0, "\\");
        }

        if (!TempSettings.Setting.ImportOverwrite && CheckTaskExists(TempSettings.Setting.ImportTaskName))
        {
            MDCustMsgBox mbox = new($"{GetStringResource("ImportTask_ImportErrorExists")} \"{TempSettings.Setting.ImportTaskName}\".",
                GetStringResource("ImportTask_ImportErrorHeader"),
                ButtonType.Ok,
                false,
                true,
                _mainWindow!,
                true);
            _ = mbox.ShowDialog();
            return;
        }

        try
        {
            using (TaskDefinition td = TaskService.Instance.NewTaskFromFile(TempSettings.Setting.ImportXMLFile))
            {
                if (TempSettings.Setting.ImportRunOnlyLoggedOn)
                {
                    td.Principal.LogonType = TaskLogonType.InteractiveToken;
                }
                if (TempSettings.Setting.ImportResetCreationDate)
                {
                    td.RegistrationInfo.Date = DateTime.Now;
                }

                _ = TaskService.Instance.RootFolder.RegisterTaskDefinition(TempSettings.Setting.ImportTaskName, td);
            }

            GetAllTasks();

            _log.Info($"Imported {TempSettings.Setting.ImportXMLFile} to \"{TempSettings.Setting.ImportTaskName}\"");
            SnackbarMsg.ClearAndQueueMessage($"{TempSettings.Setting.ImportXMLFile} {GetStringResource("ImportTask_ImportSuccess")}");
            MDCustMsgBox mbox = new($"{TempSettings.Setting.ImportXMLFile} {GetStringResource("ImportTask_ImportSuccess")}",
                    GetStringResource("ImportTask_ImportSuccessHeader"),
                    ButtonType.Ok,
                    false,
                    true,
                    _mainWindow!,
                    false);
            _ = mbox.ShowDialog();

            if (TempSettings.Setting.ImportAddToMyTasks)
            {
                AllTasks task = new()
                {
                    TaskPath = TempSettings.Setting.ImportTaskName
                };
                _ = AddTasksViewModel.AddToMyTasks(task);
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error Importing {TempSettings.Setting.ImportXMLFile}");
            MDCustMsgBox mbox = new($"{GetStringResource("ImportTask_ImportErrorGeneral")}\n\n{ex.Message}",
                    GetStringResource("ImportTask_ImportErrorHeader"),
                    ButtonType.Ok,
                    false,
                    true,
                    _mainWindow!,
                    true);
            _ = mbox.ShowDialog();
        }
    }

    /// <summary>
    /// Display a message box
    /// </summary>
    internal static void ImportCaution()
    {
        MDCustMsgBox mbox = new($"{GetStringResource("ImportTask_Caution")}",
        AppInfo.AppProduct,
        ButtonType.Ok,
        false,
        true,
        _mainWindow!,
        false);
        _ = mbox.ShowDialog();
    }
    #endregion Import a task

    #region Delete tasks
    /// <summary>
    /// Delete tasks from Windows Task Scheduler, remove it from MyTasks and then save the file.
    /// </summary>
    /// <param name="grid">Name of the DataGrid</param>
    internal static void DeleteTasks(DataGrid grid)
    {
        if (grid.SelectedItems.Count == 0)
        {
            SystemSounds.Beep.Play();
            SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_DeleteNoneSelected"), 5000);
            return;
        }
        bool deleted = false;
        for (int i = grid.SelectedItems.Count - 1; i >= 0; i--)
        {
            ScheduledTask? task = grid.SelectedItems[i] as ScheduledTask;
            try
            {
                using TaskService ts = TaskService.Instance;
                Task taskToDelete = ts.GetTask(task!.TaskPath);

                ts.RootFolder.DeleteTask(taskToDelete.Path, true);
                deleted = true;
                string msg = string.Format(GetStringResource("MsgText_Deleted"), task.TaskPath);
                SnackbarMsg.QueueMessage(msg, 2000);
                _log.Info($"Deleted: \"{task.TaskPath}\"");
                _ = ScheduledTask.TaskList.Remove(task);
                _log.Info($"Removed: \"{task.TaskPath}\"");
            }
            catch (Exception ex)
            {
                SystemSounds.Beep.Play();
                string msg = string.Format(GetStringResource("MsgText_DeleteError"), task!.TaskPath);
                SnackbarMsg.ClearAndQueueMessage($"{msg} {GetStringResource("MsgText_SeeLogFile")}", 5000);
                _log.Error(ex, $"Error attempting to delete {task.TaskPath}");
                MDCustMsgBox mbox = new($"{msg} {GetStringResource("MsgText_SeeLogFile")}",
                        GetStringResource("ImportTask_ImportErrorHeader"),
                        ButtonType.Ok,
                        false,
                        true,
                        _mainWindow!,
                        true);
                _ = mbox.ShowDialog();
            }
        }

        DialogHost.Close("MainDialogHost");
        if (deleted)
        {
            UpdateMyTasksCollection();
            TaskFileHelpers.WriteTasksToFile();
        }
    }
    #endregion Delete tasks

    #region Verify task exists
    /// <summary>
    /// Verify that the task exists in Windows Task Scheduler.
    /// </summary>
    /// <param name="taskPath">Task name including any folder name</param>
    /// <returns><see langword="true"/> if the task exists</returns>
    private static bool CheckTaskExists(string taskPath)
    {
        TaskService ts = TaskService.Instance;
        Task task = ts.GetTask(taskPath);
        ts.Dispose();
        return task != null;
    }
    #endregion Verify task exists

    #region Task note changed
    /// <summary>
    /// Change the IsDirty property when the task note changes.
    /// </summary>
    public static void TaskNoteChanged()
    {
        if (_mainWindow!.IsLoaded && !MyTasks.IsDirty)
        {
            MyTasks.IsDirty = true;
        }
    }
    #endregion Task note changed

    #region Task alert changed
    /// <summary>
    /// Change the IsDirty property when the task alert changes.
    /// </summary>
    public static void TaskAlertChanged()
    {
        if (_mainWindow!.IsLoaded)
        {
            MyTasks.IsDirty = true;
        }
    }
    #endregion Task alert changed

    #region Datagrid rows changed
    /// <summary>
    /// Change the IsDirty property after DataGrid rows are changed.
    /// </summary>
    public static void UpdateMyTasksAfterDrop()
    {
        System.Threading.Tasks.Task.Delay(100).Wait();
        UpdateMyTasksCollection();
        if (!MyTasks.IsDirty)
        {
            MyTasks.IsDirty = true;
        }
    }
    #endregion Datagrid rows changed

    #region IsDirty changed
    /// <summary>
    /// Update MyTasks list and save the file.
    /// </summary>
    public static void IsDirtyChanged()
    {
        if (!MyTasks.IsDirty || MyTasks.IgnoreChanges)
        {
            return;
        }
        MyTasks.MyTasksCollection!.Clear();
        for (int i = 0; i < ScheduledTask.TaskList.Count; i++)
        {
            ScheduledTask item = ScheduledTask.TaskList[i];
            if (item.TaskPath is not null)
            {
                MyTasks.MyTasksCollection.Add(new MyTasks(item.TaskPath, item.IsChecked, item.TaskNote));
            }
        }
        TaskFileHelpers.WriteTasksToFile();
    }
    #endregion IsDirty changed
}
