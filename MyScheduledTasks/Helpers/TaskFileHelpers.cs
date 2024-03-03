// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class TaskFileHelpers
{
    #region Tasks filename
    public static string TasksFile { get; } = Path.Combine(AppInfo.AppDirectory, "MyTasks.json");
    #endregion Tasks filename

    #region JSON serializer options
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };
    #endregion JSON serializer options

    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Read the tasks JSON file
    public static void ReadMyTasks()
    {
        // If the file doesn't exist, create a minimal JSON file
        if (!File.Exists(TasksFile))
        {
            CreateEmptyFile();
        }
        // Read the JSON file
        try
        {
            string json = File.ReadAllText(TasksFile);
            MyTasks.MyTasksCollection = JsonSerializer.Deserialize<ObservableCollection<MyTasks>>(json);
            _log.Info($"Read {MyTasks.MyTasksCollection.Count} items from {TasksFile} ");
        }
        // Can't really do much if the file is not readable
        catch (Exception ex)
        {
            _log.Fatal(ex, $"Error reading {TasksFile}");
            string msg = string.Format($"{GetStringResource("MsgText_ErrorReadingFile")}", TasksFile);
            msg += $"\n\n{ex.Message}\n\n{GetStringResource("MsgText_ErrorFatal")}";
            _ = new MDCustMsgBox(msg,
                                GetStringResource("MsgText_ErrorCaption"),
                                ButtonType.Ok,
                                true,
                                true,
                                null,
                                true).ShowDialog();

            // Quit via Environment.Exit so that normal shutdown processing doesn't run
            Environment.Exit(1);
        }
    }

    private static void CreateEmptyFile()
    {
        const string x = "[]";
        try
        {
            File.WriteAllText(TasksFile, x);
        }
        catch (Exception ex)
        {
            _log.Fatal(ex, $"Error creating {TasksFile}");
            string msg = string.Format($"{GetStringResource("MsgText_ErrorCreatingFile")}", TasksFile);
            msg += $"\n\n{ex.Message}\n\n{GetStringResource("MsgText_ErrorFatal")}";
            _ = new MDCustMsgBox(msg,
                                GetStringResource("MsgText_ErrorCaption"),
                                ButtonType.Ok,
                                true,
                                true,
                                null,
                                true).ShowDialog();

            // Quit via Environment.Exit so that normal shutdown processing doesn't run
            Environment.Exit(1);
        }
    }
    #endregion Read the tasks JSON file

    #region Write the tasks JSON file
    /// <summary>
    /// Convert MyTasksCollection to JSON and save it to a file
    /// </summary>
    /// <param name="quiet">Setting quiet to true will not display snack bar message</param>
    public static void WriteTasksToFile(bool quiet = true)
    {
        try
        {
            string tasks = JsonSerializer.Serialize(MyTasks.MyTasksCollection, _options);
            File.WriteAllText(TasksFile, tasks);
            _log.Info($"Saving {MyTasks.MyTasksCollection.Count} tasks to {TasksFile} ");
            if (!quiet)
            {
                SnackbarMsg.QueueMessage(GetStringResource("MsgText_FileSaved"), 3000);
            }
            MyTasks.IsDirty = false;
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error saving {TasksFile}");
            string msg = string.Format($"{GetStringResource("MsgText_ErrorSavingFile")}", TasksFile);
            msg += $"\n\n{ex.Message}";
            _ = new MDCustMsgBox(msg,
                                GetStringResource("MsgText_ErrorCaption"),
                                ButtonType.Ok,
                                true,
                                true,
                                _mainWindow,
                                true).ShowDialog();
        }
    }
    #endregion Write the tasks JSON file

    #region Check for empty task list
    public static void CheckEmptyList()
    {
        if (ScheduledTask.TaskList.Count == 0)
        {
            _ = new MDCustMsgBox(GetStringResource("MsgText_ErrorEmptyTaskList"),
                                   GetStringResource("MsgText_ErrorEmptyTaskListCaption"),
                                   ButtonType.YesNo,
                                   true,
                                   true,
                                   _mainWindow,
                                   false)
                                   .ShowDialog();

            if (MDCustMsgBox.CustResult == CustResultType.Yes)
            {
                _mainWindow.NavigationListBox.SelectedValue = NavigationViewModel.FindNavPage(NavPage.AddTasks);
            }
        }
    }
    #endregion Check for empty task list
}
