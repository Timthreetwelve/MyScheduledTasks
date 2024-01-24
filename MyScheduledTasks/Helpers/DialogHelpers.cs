// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class DialogHelpers
{
    /// <summary>
    /// Shows the Add Tasks dialog.
    /// </summary>
    internal static async void ShowAddTasksDialog()
    {
        MainWindowUIHelpers.MainWindowWaitPointer();
        AddTaskDialog add = new();
        _ = await DialogHost.Show(add, "MainDialogHost");
        MainWindowUIHelpers.MainWindowNormalPointer();
    }
}
