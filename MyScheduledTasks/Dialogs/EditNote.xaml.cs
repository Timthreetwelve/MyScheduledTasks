// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Dialogs;

/// <summary>
/// Dialog used to edit the TaskNote property
/// </summary>
public partial class EditNote : UserControl
{
    public EditNote(ScheduledTask task)
    {
        InitializeComponent();

        DataContext = task;
    }
}
