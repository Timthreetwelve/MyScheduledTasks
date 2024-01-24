// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Models;

internal partial class AllTasks : ObservableObject
{
    public static ObservableCollection<AllTasks> AllTasksCollection { get; set; } = [];

    [ObservableProperty]
    private string _taskName;

    [ObservableProperty]
    private string _taskPath;
}
