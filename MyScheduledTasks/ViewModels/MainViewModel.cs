// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    #region Load the task list
    /// <summary>
    /// Load the task list from MyTasksCollection
    /// </summary>
    public static void LoadData()
    {
        CheckTaskFile(MyTasks.MyTasksCollection!);

        MyTasks.IgnoreChanges = true;

        int added = 0;
        ScheduledTask.TaskList.Clear();
        for (int i = 0; i < MyTasks.MyTasksCollection!.Count; i++)
        {
            MyTasks item = MyTasks.MyTasksCollection[i];
            Task task = TaskInfo.GetTaskInfo(item.TaskPath);
            if (task is not null)
            {
                ScheduledTask schedTask = ScheduledTask.BuildScheduledTask(task, item);
                ScheduledTask.TaskList.Add(schedTask);
                _log.Debug($"Added {i + 1,3}: \"{item.TaskPath}\"");
            }
            else
            {
                _log.Warn($"No information found for \"{item.TaskPath}\"");
                string msg = $"{GetStringResource("MsgText_NoInformationFound")} {item.TaskPath}.";
                SnackbarMsg.QueueMessage(msg, 5000);

                ScheduledTask emptyTask = new()
                {
                    TaskName = item.TaskPath,
                    TaskDescription = GetStringResource("MsgText_ErrorTaskNull"),
                    TaskStatus = GetStringResource("TaskResult_Null")
                };

                ScheduledTask.TaskList.Add(emptyTask);
                added++;
                _log.Debug($"Added {added,3}: \"{item.TaskPath}\"");
            }
        }
        MyTasks.IgnoreChanges = false;
    }
    #endregion Load the task list

    #region Check tasks file
    /// <summary>
    /// Checks TaskPath for each entry in the tasks file for null or empty values.
    /// If found they will be removed from the collection and the tasks file will be rewritten.
    /// </summary>
    /// <param name="tasks">MyTasks Collection</param>
    private static void CheckTaskFile(ObservableCollection<MyTasks> tasks)
    {
        int removed = 0;
        for (int i = 0; i < tasks.Count; i++)
        {
            if (string.IsNullOrEmpty(tasks[i].TaskPath))
            {
                _log.Warn($"Null or empty item found while reading {TaskFileHelpers.TasksFile}. Item will be removed. ");
                MyTasks.MyTasksCollection!.RemoveAt(i);
                removed++;
            }
        }
        if (removed > 0)
        {
            TaskFileHelpers.WriteTasksToFile();
        }
    }
    #endregion Check tasks file
}
