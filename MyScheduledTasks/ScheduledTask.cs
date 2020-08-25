// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
#endregion

namespace MyScheduledTasks
{
    public class ScheduledTask : INotifyPropertyChanged
    {
        #region private backing fields
        private bool isChecked;
        private int taskResult;
        private string taskName;
        private string taskStatus;
        private string taskRunLevel;
        private string taskTriggers;
        private DateTime? lastRun;
        private DateTime? nextRun;
        private string taskNote;
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

        public string TaskNote
        {
            get { return taskNote; }
            set
            {
                taskNote = value;
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
                    return $"0x{TaskResult:X}";
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

        public string TaskRunLevel
        {
            get => taskRunLevel;

            set
            {
                if (value == null)
                {
                    taskRunLevel = string.Empty;
                }
                else if (value == "LUA")
                {
                    taskRunLevel = "Lowest";
                }
                else if (value == "Highest")
                {
                    taskRunLevel = value;
                }
                else
                {
                    taskRunLevel = value;
                }
            }
        }

        public string TaskTriggers
        {
            get { return taskTriggers; }
            set
            {
                if (value == null)
                {
                    taskRunLevel = string.Empty;
                }
                else
                {
                    taskTriggers = value;
                }
            }
        }

        #endregion

        #region Observable collection
        public static ObservableCollection<ScheduledTask> TaskList { get; set; }
        #endregion

        #region Property changed event handler
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
