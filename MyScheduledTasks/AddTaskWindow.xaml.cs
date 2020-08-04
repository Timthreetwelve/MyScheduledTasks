// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using Microsoft.Win32.TaskScheduler;
using System.Windows;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace MyScheduledTasks
{
    public partial class AddTaskWindow : Window
    {
        public AddTaskWindow()
        {
            InitializeComponent();
            tbAddTask.Focus();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(tbAddTask.Text))
            {
                bool a = (bool)cbxAlert.IsChecked;

                string t = tbAddTask.Text.Trim();
                if (t[0] != '\\')
                {
                    t = "\\" + t;
                }
                Task task = MainWindow.GetTaskInfo(t);

                if (task == null)
                {
                    _ = MessageBox.Show($"The Scheduled Task \"{t}\" was not found.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                ScheduledTask schedTask = new ScheduledTask
                {
                    TaskName = task.Name,
                    TaskStatus = task.State.ToString(),
                    TaskResult = task.LastTaskResult,
                    LastRun = task.LastRunTime,
                    NextRun = task.NextRunTime,
                    TaskPath = task.Path,
                    TaskMissedRuns = task.NumberOfMissedRuns,
                    IsChecked = a
                };
                ScheduledTask.TaskList.Add(schedTask);

                MyTasks my = new MyTasks(task.Path, a);
                MyTasks.MyTasksCollection.Add(my);

                tbAddTask.Text = string.Empty;
                tbAddTask.Focus();
            }
        }

        private void BtnExitAdd_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
