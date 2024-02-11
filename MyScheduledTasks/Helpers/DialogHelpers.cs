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

    /// <summary>
    /// Shows the Edit Note dialog
    /// </summary>
    /// <param name="task">task to edit note property</param>
    internal static async void ShowEditNoteDialog(ScheduledTask task)
    {
        double newSize = ScaleDialog(UserSettings.Setting.UISize);
        EditNote en = new(task)
        {
            LayoutTransform = new ScaleTransform(newSize, newSize)
        };
        _ = await DialogHost.Show(en, "MainDialogHost");
    }

    internal static double ScaleDialog(MySize size)
}
