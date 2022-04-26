// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Dialogs;

/// <summary>
/// A dialog to display a message with an OK and Cancel buttons.
/// </summary>
public partial class OkCancelDialog : UserControl
{
    /// <summary>
    /// Message to be displayed
    /// </summary>
    public string Message { get; set; }

    public OkCancelDialog()
    {
        InitializeComponent();
        DataContext = this;
    }
}
