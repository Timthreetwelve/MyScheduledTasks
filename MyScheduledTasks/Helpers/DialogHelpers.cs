// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Helpers;

internal static class DialogHelpers
{
    #region Import dialog
    /// <summary>
    /// Shows the Import Tasks dialog.
    /// </summary>
    internal static async void ShowImportTaskDialog()
    {
        MainWindowUIHelpers.MainWindowNotAllowedPointer();
        ImportTaskDialog importDialog = new();
        _ = await DialogHost.Show(importDialog, "MainDialogHost");
        MainWindowUIHelpers.MainWindowNormalPointer();
    }
    #endregion Import dialog

    #region Delete dialog
    /// <summary>
    /// Shows the Delete Tasks dialog.
    /// </summary>
    internal static async void ShowDeleteTasksDialog(DataGrid grid)
    {
        MainWindowUIHelpers.MainWindowNotAllowedPointer();
        DeleteTasksDialog deleteDialog = new(grid);
        _ = await DialogHost.Show(deleteDialog, "MainDialogHost");
        MainWindowUIHelpers.MainWindowNormalPointer();
    }
    #endregion Delete dialog

    #region Edit note dialog
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
    #endregion Edit note dialog
}
