// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Models;

internal sealed partial class AllTasks : ObservableObject
{
    public static ObservableCollection<AllTasks> All_TasksCollection { get; } = [];
    public static ObservableCollection<AllTasks> Non_MS_TasksCollection { get; } = [];

    [ObservableProperty]
    private string? _taskPath;

    [ObservableProperty]
    private string? _taskName;

    [ObservableProperty]
    private string? _taskFolder;
}
