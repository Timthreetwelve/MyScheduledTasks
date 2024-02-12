// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class DialogHelpers
{
    /// <summary>
    /// Shows the Add Tasks dialog.
    /// </summary>
    internal static async void ShowImportTaskDialog()
    {
        MainWindowUIHelpers.MainWindowNotAllowedPointer();
        ImportTaskDialog importDialog = new();
        _ = await DialogHost.Show(importDialog, "MainDialogHost");
        MainWindowUIHelpers.MainWindowNormalPointer();
    }

    /// <summary>
    /// Shows the Edit Note dialog
    /// </summary>
    /// <param name="task">task to edit note property</param>
    internal static async void ShowEditNoteDialog(ScheduledTask task)
    {
        MainWindowUIHelpers.MainWindowNotAllowedPointer();
        EditNote en = new(task);
        _ = await DialogHost.Show(en, "MainDialogHost");
        MainWindowUIHelpers.MainWindowNormalPointer();
    }
}
