// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
#endregion

namespace MyScheduledTasks
{
    public class ScheduledTask : INotifyPropertyChanged
    {
        #region private backing fields
        private bool isChecked;
        private string taskName;
        private string taskStatus;
        private int taskResult;
        private DateTime? lastRun;
        private DateTime? nextRun;
        #endregion

        #region Properties
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged();
            }
        }

        public string TaskName
        {
            get => taskName;
            set
            {
                if (value != null)
                {
                    taskName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TaskStatus
        {
            get => taskStatus;
            set
            {
                if (value != null)
                {
                    taskStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public int? TaskResult
        {
            get => taskResult;
            set
            {
                if (value != null)
                {
                    taskResult = (int)value;
                    OnPropertyChanged();
                }
            }
        }

        public string TaskResultShort
        {
            get
            {
                if (TaskResult == null)
                {
                    return string.Empty;
                }
                else if (TaskResult == 0)
                {
                    return "OK";
                }
                else
                {
                    return "NZ";
                }
            }
        }

        public string TaskResultHex
        {
            get
            {
                if (TaskResult == null)
                {
                    return string.Empty;
                }
                else
                {
                    string hex = $"0x{TaskResult:X}";
                    return hex;
                }
            }
        }

        public DateTime? LastRun
        {
            get => lastRun;

            set
            {
                if (value == DateTime.MinValue)
                {
                    lastRun = null;
                }
                else
                {
                    lastRun = value;
                }
            }
        }

        public DateTime? NextRun
        {
            get => nextRun;

            set
            {
                if (value == DateTime.MinValue)
                {
                    nextRun = null;
                }
                else
                {
                    nextRun = value;
                }
            }
        }

        public string TaskFolder { get; set; }

        public string TaskPath { get; set; }

        public int TaskMissedRuns { get; set; }

        public string TaskAccount { get; set; }

        public string TaskDescription { get; set; }

        public string TaskAuthor { get; set; }

        #endregion

        #region Observable collection
        public static ObservableCollection<ScheduledTask> TaskList { get; set; }
        #endregion

        #region Property changed event handler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            Debug.WriteLine($"+++ Property changed: {propertyName}");
        }
        #endregion
    }
}
