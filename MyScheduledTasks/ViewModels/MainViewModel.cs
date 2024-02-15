// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.ViewModels;

internal class MainViewModel
{
    #region Constructor
    public MainViewModel()
    {
    }
    #endregion Constructor

    #region Load the task list
    /// <summary>
    /// Load the task list from MyTasksCollection
    /// </summary>
    public static void LoadData()
    {
        bool hasBadRecord = false;
        ScheduledTask.TaskList.Clear();
        //ObservableCollection<ScheduledTask> taskList = [];
        BindingList<ScheduledTask> bindingList = [];
        for (int i = 0; i < MyTasks.MyTasksCollection.Count; i++)
        {
            MyTasks item = MyTasks.MyTasksCollection[i];
            if (string.IsNullOrEmpty(item.TaskPath))
            {
                _log.Warn($"Null or empty item found in position {i + 1} while reading {TaskFileHelpers.TasksFile}");
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
                    bindingList.Add(schedTask);
                    _log.Debug($"Added {i + 1,3}: \"{item.TaskPath}\"");
                }
                else
                {
                    _log.Warn($"No information found for \"{item.TaskPath}\"");
                    string msg = $"{GetStringResource("MsgText_NoInformationFound")} {item.TaskPath}.";
                    SnackbarMsg.QueueMessage(msg, 5000);

                    ScheduledTask emptyTask = new()
                    {
                        TaskName = item.TaskPath
                    };
                    if (!AppInfo.IsAdmin)
                    {
                        emptyTask.TaskDescription = "No information for this task was returned. Try running My Scheduled Tasks as Administrator. ";
                        emptyTask.TaskStatus = GetStringResource("TaskResult_Null");
                    }
                    else
                    {
                        emptyTask.TaskDescription = "No information for this task was returned. The task may no longer exist. Check Task Scheduler.";
                        emptyTask.TaskStatus = GetStringResource("TaskResult_Null");
                    }
                    ScheduledTask.TaskList.Add(emptyTask);
                    bindingList.Add(emptyTask);
                    _log.Debug($"Added {i + 1,3}: \"{item.TaskPath}\"");
                }
            }
        }

        //ScheduledTask.TaskList = taskList;
        ScheduledTask.TaskList.CollectionChanged += TaskList_CollectionChanged;
        bindingList.ListChanged += Binding_ListChanged;
        if (hasBadRecord)
        {
            //TaskFileHelpers.WriteTasks2Json();
        }
    }

    private static void Binding_ListChanged(object sender, ListChangedEventArgs e)
    {
        //UpdateMyTasksCollection();
        //TaskFileHelpers.WriteTasks2Json(quiet: true);
    }

    private static void TaskList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        //UpdateMyTasksCollection();
        //TaskFileHelpers.WriteTasks2Json(quiet: true);
    }
    #endregion Load the task list

    #region Get updated tasks from TaskList
    public static void UpdateMyTasksCollection()
    {
        MyTasks.MyTasksCollection.Clear();
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
}
