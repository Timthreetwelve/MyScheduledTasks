// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Views;

public partial class MainPage : UserControl
{
    #region MainPage Instance
    public static MainPage Instance { get; private set; }
    #endregion MainPage Instance

    #region Constructor
    public MainPage()
    {
        InitializeComponent();

        Instance = this;

        // The following fixes binding ElementName in the Context Menu.
        // Credit: https://stackoverflow.com/a/1066009
        NameScope.SetNameScope(DGContextMenu, NameScope.GetNameScope(this));

        // Details pane size
        detailsRow.Height = !UserSettings.Setting.ShowDetails
            ? new GridLength(1)
            : new GridLength(UserSettings.Setting.DetailsHeight);
    }
    #endregion Constructor

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

    #region GridSplitter drag completed
    private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
    {
        UserSettings.Setting.DetailsHeight = detailsRow.Height.Value;
    }
    #endregion GridSplitter drag completed

    #region DataGrid row drag & drop
    public void DataGridTasksDrop(object sender, DragEventArgs e)
    {
        if (e.Source == DataGridTasks)
        {
            _ = System.Threading.Tasks.Task.Run(TaskHelpers.UpdateMyTasksAfterDrop);
        }
    }
    #endregion DataGrid row drag & drop
}
