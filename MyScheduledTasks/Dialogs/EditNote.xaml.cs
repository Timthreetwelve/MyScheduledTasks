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

        Loaded += (_, _) =>
        {
            NoteTextBox.CaretIndex = NoteTextBox.Text.Length;
            NoteTextBox.ScrollToEnd();
            NoteTextBox.Focus();
        };
    }

    #region TextBox key down event
    /// <summary>Handles the KeyDown event of the TextBox control.</summary>
    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        // Update property when enter is pressed
        if (e.Key == Key.Enter)
        {
            // https://stackoverflow.com/a/13289118
            TextBox tBox = (TextBox)sender;
            DependencyProperty prop = TextBox.TextProperty;
            BindingExpression? binding = BindingOperations.GetBindingExpression(tBox, prop);
            binding?.UpdateSource();
        }
    }
    #endregion
}
