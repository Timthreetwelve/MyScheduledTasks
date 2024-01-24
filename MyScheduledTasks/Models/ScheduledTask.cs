// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Models;

public class ScheduledTask : INotifyPropertyChanged
{
    #region Public Methods
    /// <summary>
    /// Creates a ScheduledTask object
    /// </summary>
    /// <param name="task">TaskScheduler task</param>
    /// <param name="myTask">Task from list of tasks to check</param>
    /// <returns>ScheduledTask object</returns>
    public static ScheduledTask BuildScheduledTask(Task task, MyTasks myTask)
    {
        if (task != null)
        {
            string folder = task.Path.Replace(task.Name, "");
            if (folder == "\\")
            {
                folder = "\\  (root)";
            }
            return new()
            {
                TaskName = task.Name,
                TaskStatus = task.State.ToString(),
                TaskResult = (uint?)task.LastTaskResult,
                LastRun = task.LastRunTime,
                NextRun = task.NextRunTime,
                TaskPath = task.Path,
                TaskFolder = folder,
                TaskActions = task.Definition.Actions.ToString(),
                TaskMissedRuns = task.NumberOfMissedRuns,
                TaskAccount = task.Definition.Principal.Account,
                TaskRunLevel = task.Definition.Principal.RunLevel.ToString(),
                TaskDescription = task.Definition.RegistrationInfo.Description,
                TaskAuthor = task.Definition.RegistrationInfo.Author,
                TaskTriggers = task.Definition.Triggers.ToString(),
                Enabled = task.Definition.Settings.Enabled,
                Priority = task.Definition.Settings.Priority.ToString(),
                AllowDemandStart = task.Definition.Settings.AllowDemandStart,
                TimeLimit = task.Definition.Settings.ExecutionTimeLimit.ToString(),
                WakeToRun = task.Definition.Settings.WakeToRun,
                TaskNote = myTask != null ? myTask.TaskNote : string.Empty,
                IsChecked = myTask?.Alert == true
            };
        }
        return null;
    }
    #endregion Public Methods

    #region Private backing fields
    private bool _isChecked;
    private uint? _taskResult;
    private string _taskActions;
    private string _taskName;
    private string _taskStatus;
    private string _taskRunLevel;
    private string _taskTriggers;
    private DateTime? _lastRun;
    private DateTime? _nextRun;
    private string _taskNote;

    #endregion Private backing fields

    #region Properties
    public string TimeLimit { get; set; }

    public bool AllowDemandStart { get; set; }

    public bool WakeToRun { get; set; }

    public bool Enabled { get; set; }

    public string Priority { get; set; }

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            _isChecked = value;
            OnPropertyChanged();
        }
    }

    public string TaskNote
    {
        get { return _taskNote; }
        set
        {
            _taskNote = value;
            OnPropertyChanged();
        }
    }

    public string TaskName
    {
        get => _taskName;
        set
        {
            if (value != null)
            {
                _taskName = value;
                OnPropertyChanged();
            }
        }
    }

    public string TaskStatus
    {
        get => _taskStatus;
        set
        {
            if (value != null)
            {
                _taskStatus = value;
                OnPropertyChanged();
            }
        }
    }

    public uint? TaskResult
    {
        get => _taskResult;
        set
        {
            if (value != null)
            {
                _taskResult = value;
                OnPropertyChanged();
            }
        }
    }

    public string TaskResultShort
    {
        get
        {
            switch (TaskResult)
            {
                case null:
                    return string.Empty;
                case 0:
                    return "OK";
                case 0x41300:
                    return "RDY"; //Task is ready to run at its next scheduled time
                case 0x41301:
                    return "RUN"; //The task is currently running
                case 0x41302:
                    return "DIS"; //The task has been disabled
                case 0x41303:
                    return "NYR"; //The task has not yet run
                case 0x41304:
                    return "NMR"; //There are no more runs scheduled for this task
                case 0x41306:
                    return "TRM"; //The last run of the task was terminated by the user
                case 0x8004131F:
                    return "AR"; //An instance of this task is already running
                case 0x80070002:
                    return "FNF"; //File not found
                default:
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
        get => _lastRun;

        set
        {
            if (value == DateTime.MinValue)
            {
                _lastRun = null;
            }
            else
            {
                _lastRun = value;
            }
        }
    }

    public DateTime? NextRun
    {
        get => _nextRun;

        set
        {
            if (value == DateTime.MinValue)
            {
                _nextRun = null;
            }
            else
            {
                _nextRun = value;
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
        get => _taskRunLevel;

        set
        {
            if (value == null)
            {
                _taskRunLevel = string.Empty;
            }
            else if (value == "LUA")
            {
                _taskRunLevel = "Lowest";
            }
            else if (value == "Highest")
            {
                _taskRunLevel = value;
            }
            else
            {
                _taskRunLevel = value;
            }
        }
    }

    public string TaskTriggers
    {
        get { return _taskTriggers; }
        set
        {
            _taskTriggers = value ?? string.Empty;
        }
    }

    public string TaskActions
    {
        get { return _taskActions; }
        set
        {
            _taskActions = value ?? string.Empty;
        }
    }

    #endregion Properties

    #region Observable collection
    public static ObservableCollection<ScheduledTask> TaskList { get; set; }
    #endregion Observable collection

    #region Property changed event handler
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion Property changed event handler
}
