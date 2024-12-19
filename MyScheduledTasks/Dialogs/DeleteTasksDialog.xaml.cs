// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Dialogs;

/// <summary>
/// Interaction logic for DeleteTasksDialog.xaml
/// </summary>
public partial class DeleteTasksDialog : UserControl
{
    public DeleteTasksDialog()
    {
        InitializeComponent();
    }

    private void UserControl_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            DialogHost.Close("MainDialogHost");
            e.Handled = true;
        }
    }
}
