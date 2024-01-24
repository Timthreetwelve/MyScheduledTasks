﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Models;

public partial class MyTasks : ObservableObject
{
    #region Constructor
    public MyTasks(string taskPath, bool alert, string taskNote)
    {
        TaskPath = taskPath;
        Alert = alert;
        TaskNote = taskNote;
    }
    #endregion Constructor

    #region Properties
    [ObservableProperty]
    private string _taskPath;

    [ObservableProperty]
    private bool _alert;

    [ObservableProperty]
    private string _taskNote;

    public static bool IsDirty { get; set; }

    public static bool SortIsDirty { get; set; }
    #endregion Properties

    #region Observable collection
    public static ObservableCollection<MyTasks> MyTasksCollection { get; set; }
    #endregion Observable collection
}
