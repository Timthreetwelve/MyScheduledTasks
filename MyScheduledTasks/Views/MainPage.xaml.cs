// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Views;

/// <summary>
/// Interaction logic for MainPage.xaml
/// </summary>
public partial class MainPage : UserControl
{
    public static MainPage Instance { get; private set; }

    public MainPage()
    {
        InitializeComponent();

        Instance = this;

        // Details pane size
        detailsRow.Height = !UserSettings.Setting.ShowDetails
            ? new GridLength(1)
            : new GridLength(UserSettings.Setting.DetailsHeight);
    }

    #region Clear column sort
    /// <summary>
    ///  Clears any sorts that may have been applied to columns in the data grid
    /// </summary>
    internal void ClearColumnSort()
    {
        foreach (DataGridColumn column in DataGridTasks.Columns)
        {
            column.SortDirection = null;
        }
        DataGridTasks.Items.SortDescriptions.Clear();

        SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_ColumnSortCleared"));
    }
    #endregion Clear column sort

    private void MenuOpened(object sender, RoutedEventArgs e)
    {
        Debug.WriteLine("Why?");
    }

    private void DataGridTasks_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
    }

    private void DataGridTasks_Sorting(object sender, DataGridSortingEventArgs e)
    {
    }

    private void DataGridTasks_Drop(object sender, DragEventArgs e)
    {
    }

    #region Context menu opened event
    private void ContextMenu_Opened(object sender, RoutedEventArgs e)
    {
        EditNoteItem.IsEnabled = DataGridTasks.SelectedItems.Count == 1;
    }
    #endregion Context menu opened event
}
