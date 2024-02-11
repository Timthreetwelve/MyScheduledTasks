﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Views;

public partial class AddTasks : UserControl
{
    public static AddTasks Instance { get; private set; }

    public AddTasks()
    {
        InitializeComponent();

        Instance = this;

        AddTasksViewModel.HideTasks(AllTasksGrid);
        AddTasksViewModel.StaticPropertyChanged += AddTasksViewModel_StaticPropertyChanged;
    }

    private void AddTasksViewModel_StaticPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        FilterTheGrid(AddTasksViewModel.FilterText);
    }

    #region Filter the list
    #endregion Filter the list

    #region Filter the datagrid
    /// <summary>
    /// Filters the grid.
    /// </summary>
    public void FilterTheGrid(string filterText)
    {
        if (string.IsNullOrEmpty(filterText))
        {
            AllTasksGrid.Items.Filter = _ => true;
        }
        else if (filterText?.StartsWith('!') == true)
        {
            filterText = filterText[1..].TrimStart(' ');
            AllTasksGrid.Items.Filter = o =>
            {
                AllTasks tasks = o as AllTasks;
                return !tasks.TaskName.Contains(filterText, StringComparison.CurrentCultureIgnoreCase);
            };
        }
        else
        {
            AllTasksGrid.Items.Filter = o =>
            {
                AllTasks tasks = o as AllTasks;
                return tasks.TaskName.Contains(filterText, StringComparison.CurrentCultureIgnoreCase);
            };
        }

        if (AllTasksGrid.Items.Count == 1)
        {
            SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_FilterOneRowShown"), 2000);
        }
        else
        {
            SnackbarMsg.ClearAndQueueMessage(string.Format(
                GetStringResource("MsgText_FilterRowsShown"), AllTasksGrid.Items.Count), 2000);
        }
    }
    #endregion Filter the datagrid
}
