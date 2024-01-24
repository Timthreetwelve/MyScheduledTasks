// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class TaskFileHelpers
{
    public static string TasksFile { get; } = Path.Combine(AppInfo.AppDirectory, "MyTasks.json");

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    #region MainWindow Instance
    private static readonly MainWindow _mainWindow = Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    #region Read the tasks JSON file
    public static void ReadMyTasks()
    {
        // If the file doesn't exist, create a minimal JSON file
        if (!File.Exists(TasksFile))
        {
            const string x = "[]";
            try
            {
                File.WriteAllText(TasksFile, x);
            }
            catch (Exception ex)
            {
                _log.Fatal(ex, $"Error creating {TasksFile}");
                _ = new MDCustMsgBox($"Error creating {TasksFile}\n\n{ex.Message}",
                                    "ERROR", ButtonType.Ok).ShowDialog();

                _ = new MDCustMsgBox("Fatal Error. My Scheduled Tasks will now close.",
                                    "FATAL ERROR", ButtonType.Ok, true).ShowDialog();

                // Quit via Environment.Exit so that normal shutdown processing doesn't run
                Environment.Exit(1);
            }
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
            _ = new MDCustMsgBox($"Error reading {TasksFile}\n\n{ex.Message}",
                                "ERROR", ButtonType.Ok).ShowDialog();

            _ = new MDCustMsgBox("Fatal Error. My Scheduled Tasks will now close.",
                                "FATAL ERROR", ButtonType.Ok, true).ShowDialog();

            // Quit via Environment.Exit so that normal shutdown processing doesn't run
            Environment.Exit(1);
        }
    }
    #endregion Read the tasks JSON file

    #region Write the tasks JSON file
    /// <summary>
    /// Convert MyTasksCollection to JSON and save it to a file
    /// </summary>
    public static void WriteTasks2Json()
    {
        try
        {
            string x = JsonSerializer.Serialize(MyTasks.MyTasksCollection, _options);
            File.WriteAllText(TasksFile, x);
            _log.Info($"Writing {MyTasks.MyTasksCollection.Count} items to {TasksFile} ");
            SnackbarMsg.ClearAndQueueMessage($"{TasksFile} has been saved");
            MyTasks.IsDirty = false;
            //sbRight.Content = string.Empty;
        }
        catch (Exception ex)
        {
            _log.Error(ex, $"Error saving {TasksFile}");
            if (!DialogHost.IsDialogOpen("MainDialogHost"))
            {
                //DialogHelpers.ShowErrorDialog($"Error saving {TasksFile}\n\n{ex.Message}");
            }
            else
            {
                DialogHost.Close("MainDialogHost");
                //DialogHelpers.ShowErrorDialog($"Error saving {TasksFile}\n\n{ex.Message}");
            }
        }
    }
    #endregion Write the tasks JSON file

    #region Check for empty task list (first run)
    public static void CheckEmptyList()
    {
        if (ScheduledTask.TaskList.Count == 0)
        {
            _ = new MDCustMsgBox("The task list is empty. Would you like to add tasks now?",
                                   "ADD TASKS?",
                                   ButtonType.YesNo,
                                   true,
                                   true,
                                   _mainWindow,
                                   false)
                                   .ShowDialog();

            if (MDCustMsgBox.CustResult == CustResultType.Yes)
            {
                DialogHelpers.ShowAddTasksDialog();
            }
        }
    }
    #endregion Check for empty task list (first run)
}
