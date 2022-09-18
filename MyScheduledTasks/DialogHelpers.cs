// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks;

internal static class DialogHelpers
{
    /// <summary>
    /// Shows the Edit Note dialog
    /// </summary>
    /// <param name="task">task to edit note property</param>
    internal static async void ShowEditNoteDialog(ScheduledTask task)
    {
        EditNote en = new(task);
        _ = await DialogHost.Show(en, "MainDialogHost");
    }

    /// <summary>
    /// Shows the Add Tasks dialog.
    /// </summary>
    internal static async void ShowAddTasksDialog()
    {
        AddTaskDialog add = new();
        _ = await DialogHost.Show(add, "MainDialogHost");
    }

    /// <summary>
    /// Shows the About dialog.
    /// </summary>
    internal static async void ShowAboutDialog()
    {
        About about = new();
        _ = await DialogHost.Show(about, "MainDialogHost");
    }

    /// <summary>
    /// Shows the Settings dialog
    /// </summary>
    internal static async void ShowSettingsDialog()
    {
        Settings settings = new();
        _ = await DialogHost.Show(settings, "MainDialogHost");
    }

    /// <summary>
    /// Shows the Error dialog
    /// </summary>
    /// <param name="msg">Message text</param>
    internal static async void ShowErrorDialog(string msg)
    {
        SystemSounds.Exclamation.Play();
        ErrorDialog error = new()
        {
            Message = msg
        };
        _ = await DialogHost.Show(error, "MainDialogHost");
    }
}
