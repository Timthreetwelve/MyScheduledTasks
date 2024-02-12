// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class TaskInfo
{
    #region Get scheduled task info for one task
    /// <summary>
    /// Get scheduled task info for one task
    /// </summary>
    /// <param name="name">Name (including folder name) of scheduled task</param>
    /// <returns>Scheduled task object as Task</returns>
    public static Task GetTaskInfo(string name)
    {
        //using TaskService ts = new();

        using TaskService ts = TaskService.Instance;

        return ts.GetTask(name);
    }
    #endregion Get scheduled task info for one task
}
