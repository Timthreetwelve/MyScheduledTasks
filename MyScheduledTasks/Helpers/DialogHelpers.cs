// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class DialogHelpers
{
    #region Import dialog
    /// <summary>
    /// Shows the Import Tasks dialog.
    /// </summary>
    internal static async System.Threading.Tasks.Task ShowImportTaskDialog()
    {
        MainWindowHelpers.MainWindowNotAllowedPointer();
        ImportTaskDialog importDialog = new();
        _ = await DialogHost.Show(importDialog, "MainDialogHost");
        MainWindowHelpers.MainWindowNormalPointer();
    }
    #endregion Import dialog

    #region Delete dialog
    /// <summary>
    /// Shows the Delete Tasks dialog.
    /// </summary>
    internal static async System.Threading.Tasks.Task ShowDeleteTasksDialog()
    {
        MainWindowHelpers.MainWindowNotAllowedPointer();
        DeleteTasksDialog deleteDialog = new();
        _ = await DialogHost.Show(deleteDialog, "MainDialogHost");
        MainWindowHelpers.MainWindowNormalPointer();
    }
    #endregion Delete dialog

    #region Edit note dialog
    /// <summary>
    /// Shows the Edit Note dialog
    /// </summary>
    /// <param name="task">task to edit note property</param>
    internal static async System.Threading.Tasks.Task ShowEditNoteDialog(ScheduledTask task)
    {
        MainWindowHelpers.MainWindowNotAllowedPointer();
        EditNote en = new(task);
        _ = await DialogHost.Show(en, "MainDialogHost");
        MainWindowHelpers.MainWindowNormalPointer();
    }
    #endregion Edit note dialog
}
