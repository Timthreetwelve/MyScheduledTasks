// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Dialogs;

/// <summary>
/// A dialog to display a message with an OK button. This dialog's border and button background are
/// set to the secondary accent color.
/// </summary>
public partial class ErrorDialog : UserControl
{
    /// <summary>
    /// Message to be displayed
    /// </summary>
    public string Message { get; set; }

    public ErrorDialog()
    {
        InitializeComponent();
        DataContext = this;
    }
}
