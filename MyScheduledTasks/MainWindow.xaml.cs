// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        MainWindowHelpers.MyScheduledTasksStartUp();
    }
}
