// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks;

public class ScheduledTask : INotifyPropertyChanged
{
    #region Public Methods
    public static ScheduledTask BuildSchedTask(Task task, MyTasks item)
    {
        if (task != null)
        {
            string folder = task.Path.Replace(task.Name, "");
            if (folder == "\\")
            {
                folder = "\\  (root)";
            }
            ScheduledTask schedTask = new()
            {
                TaskName = task.Name,
                TaskStatus = task.State.ToString(),
                TaskResult = task.LastTaskResult,
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
                TaskTriggers = task.Definition.Triggers.ToString()
            };
            schedTask.TaskNote = item != null ? item.TaskNote : string.Empty;
            schedTask.IsChecked = item != null ? item.Alert : false;
            return schedTask;
        }
        return null;
    }
    #endregion Public Methods

    #region Private backing fields
    private bool isChecked;
    private int taskResult;
    private string taskName;
    private string taskStatus;
    private string taskRunLevel;
    private string taskTriggers;
    private DateTime? lastRun;
    private DateTime? nextRun;
    private string taskNote;
    #endregion Private backing fields

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
            taskTriggers = value ?? string.Empty;
        }
    }

    private string taskActions;

    public string TaskActions
    {
        get { return taskActions; }
        set
        {
            taskActions = value ?? string.Empty;
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
