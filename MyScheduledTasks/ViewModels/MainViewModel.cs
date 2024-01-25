// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.ViewModels;

internal class MainViewModel
{
    public MainViewModel()
    {
    }

    #region Load the task list
    /// <summary>
    /// Load the task list from MyTasksCollection
    /// </summary>
    public static void LoadData()
    {
        ObservableCollection<ScheduledTask> taskList = [];
        BindingList<ScheduledTask> bindingList = [];
        //ScheduledTask st = new();
        int count = 0;
        foreach (MyTasks item in MyTasks.MyTasksCollection)
        {
            Task task = TaskInfo.GetTaskInfo(item.TaskPath);

            if (task != null)
            {
                ScheduledTask schedTask = ScheduledTask.BuildScheduledTask(task, item);
                taskList.Add(schedTask);
                bindingList.Add(schedTask);
                count++;
                _log.Debug($"Added {count,3}: \"{item.TaskPath}\"");
            }
            else if (item.TaskPath == null)
            {
                _log.Warn($"Null item found while reading {TaskFileHelpers.TasksFile}");
                string msg = $"{GetStringResource("MsgText_NullItemFound")}  {GetStringResource("MsgText_SeeLogFile")}";
                SnackbarMsg.QueueMessage(msg, 5000);
            }
            else
            {
                _log.Warn($"No information found for \"{item.TaskPath}\"");
                string msg = $"{GetStringResource("MsgText_NoInformationFound")} {item.TaskPath}.  {GetStringResource("MsgText_SeeLogFile")}";
                SnackbarMsg.QueueMessage(msg, 5000);
            }
        }
        //ToDo Sort this crap out
        //DataContext = st;
        ScheduledTask.TaskList = taskList;
        //ScheduledTask.TaskList.CollectionChanged += TaskList_CollectionChanged;
        //bindingList.ListChanged += Binding_ListChanged;
        StatusBarItems.SbLeft = ScheduledTask.TaskList.Count.ToString();
    }
    #endregion Load the task list
}
