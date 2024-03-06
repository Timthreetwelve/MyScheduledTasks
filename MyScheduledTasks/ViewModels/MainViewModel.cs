// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.ViewModels;

#pragma warning disable RCS1102 // Make class static
internal class MainViewModel
#pragma warning restore RCS1102 // Make class static
{
    #region Load the task list
    /// <summary>
    /// Load the task list from MyTasksCollection
    /// </summary>
    public static void LoadData()
    {
        MyTasks.IgnoreChanges = true;
        bool hasBadRecord = false;
        ScheduledTask.TaskList.Clear();
        for (int i = 0; i < MyTasks.MyTasksCollection.Count; i++)
        {
            MyTasks item = MyTasks.MyTasksCollection[i];
            if (string.IsNullOrEmpty(item.TaskPath))
            {
                _log.Warn($"Null or empty item found in position {i + 1} while reading {TaskFileHelpers.TasksFile}. Item will be removed. ");
                MyTasks.MyTasksCollection.RemoveAt(i);
                i--;
                hasBadRecord = true;
            }
            else
            {
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
                    _log.Debug($"Added {i + 1,3}: \"{item.TaskPath}\"");
                }
            }
        }

        if (hasBadRecord)
        {
            TaskFileHelpers.WriteTasksToFile();
        }
        MyTasks.IgnoreChanges = false;
    }
    #endregion Load the task list
}
