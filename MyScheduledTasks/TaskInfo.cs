// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using Task = Microsoft.Win32.TaskScheduler.Task;

namespace MyScheduledTasks
{
    internal static class TaskInfo
    {
        #region Get scheduled task info for one task
        public static Task GetTaskInfo(string name)
        {
            using TaskService ts = new();
            return ts.GetTask(name);
        }

        #endregion Get scheduled task info for one task
    }
}
