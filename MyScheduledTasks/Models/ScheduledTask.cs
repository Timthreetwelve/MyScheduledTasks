// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Models;

public partial class ScheduledTask : ObservableObject
{
    #region Build a ScheduledTask object
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
            // Get folder name
            int pos = task.Path.LastIndexOf('\\');
            string folder = pos == 0 ? "\\" : task.Path[..pos];

            ScheduledTask scheduledTask = new()
            {
                AllowDemandStart = task.Definition.Settings.AllowDemandStart,
                Enabled = task.Definition.Settings.Enabled,
                IsChecked = myTask?.Alert == true,
                LastRunRaw = task.LastRunTime,
                NextRunRaw = task.NextRunTime,
                Priority = task.Definition.Settings.Priority.ToString(),
                StartASAP = task.Definition.Settings.StartWhenAvailable,
                StartOnAC = task.Definition.Settings.DisallowStartIfOnBatteries,
                TaskAccount = task.Definition.Principal.Account,
                TaskActions = task.Definition.Actions.ToString(),
                TaskAuthor = task.Definition.RegistrationInfo.Author,
                TaskCreatedRaw = task.Definition.RegistrationInfo.Date,
                TaskDescription = task.Definition.RegistrationInfo.Description,
                TaskFolder = folder,
                TaskMissedRuns = task.NumberOfMissedRuns,
                TaskName = task.Name,
                TaskNote = myTask != null ? myTask.TaskNote : string.Empty,
                TaskPath = task.Path,
                TaskResult = (uint?)task.LastTaskResult,
                TaskRunLevel = (int)task.Definition.Principal.RunLevel,
                TaskRunLoggedOn = task.Definition.Settings.RunOnlyIfLoggedOn,
                TaskStatus = task.State.ToString(),
                TaskTriggers = task.Definition.Triggers.ToString(),
                TimeLimit = task.Definition.Settings.ExecutionTimeLimit.ToString(),
                WakeToRun = task.Definition.Settings.WakeToRun,
            };

            if (task.Definition.Triggers.Count > 1)
            {
                IEnumerable<string> triggers = task.Definition.Triggers.Select(t => t.ToString());
                scheduledTask.TaskTriggers = string.Join(Environment.NewLine, triggers).TrimEnd(Environment.NewLine.ToCharArray());
            }
            if (task.Definition.Actions.Count > 1)
            {
                IEnumerable<string> actions = task.Definition.Actions.Select(a => a.ToString());
                scheduledTask.TaskActions = string.Join(Environment.NewLine, actions).TrimEnd(Environment.NewLine.ToCharArray());
            }

            return scheduledTask;
        }
        return null;
    }
    #endregion Build a ScheduledTask object

    #region Observable collection
    public static ObservableCollection<ScheduledTask> TaskList { get; set; } = [];
    #endregion Observable collection

    #region Properties
    [ObservableProperty]
    private bool _allowDemandStart;

    [ObservableProperty]
    private bool _enabled;

    [ObservableProperty]
    private DateTime? _lastRunRaw;

    [ObservableProperty]
    private DateTime? _nextRunRaw;

    [ObservableProperty]
    private string _priority;

    [ObservableProperty]
    private bool _startASAP;

    [ObservableProperty]
    private bool _startOnAC;

    [ObservableProperty]
    private string _taskAccount;

    [ObservableProperty]
    private string _taskActions;

    [ObservableProperty]
    private string _taskAuthor;

    [ObservableProperty]
    private DateTime? _taskCreatedRaw;

    [ObservableProperty]
    private string _taskDescription;

    [ObservableProperty]
    private string _taskFolder;

    [ObservableProperty]
    private string _timeLimit;

    [ObservableProperty]
    private int _taskMissedRuns;

    [ObservableProperty]
    private string _taskName;

    [ObservableProperty]
    private string _taskPath;

    [ObservableProperty]
    private uint? _taskResult;

    [ObservableProperty]
    private int _taskRunLevel;

    [ObservableProperty]
    private bool _taskRunLoggedOn;

    [ObservableProperty]
    private string _taskStatus;

    [ObservableProperty]
    private string _taskTriggers;

    [ObservableProperty]
    private bool _wakeToRun;

    public bool HighestPrivileges => TaskRunLevel != 0;

    public DateTime? LastRun => LastRunRaw == null || LastRunRaw == DateTime.MinValue || LastRunRaw == new DateTime(1999, 11, 30, 0, 0, 0, DateTimeKind.Local) ? null : LastRunRaw;

    public DateTime? NextRun => NextRunRaw == DateTime.MinValue ? null : NextRunRaw;

    public DateTime? TaskCreated => TaskCreatedRaw == DateTime.MinValue ? null : TaskCreatedRaw;

    public string TaskResultHex => (TaskResult == null) ? string.Empty : $"0x{TaskResult:X8}";

    public string TaskResultShort
    {
        get
        {
            return TaskResult switch
            {
                null => GetStringResource("TaskResult_Null"),               //Null
                0 => GetStringResource("TaskResult_OK"),                    //The operation completed successfully
                0x41300 => GetStringResource("TaskResult_ReadyToRun"),      //Task is ready to run at its next scheduled time
                0x41301 => GetStringResource("TaskResult_Running"),         //The task is currently running
                0x41302 => GetStringResource("TaskResult_Disabled"),        //The task has been disabled
                0x41303 => GetStringResource("TaskResult_NotYetRun"),       //The task has not yet run
                0x41306 => GetStringResource("TaskResult_Terminated"),      //The last run of the task was terminated by the user
                0x80070002 => GetStringResource("TaskResult_FileNotFound"), //File not found
                _ => GetStringResource("TaskResult_NonZero"),               //Other non-zero
            };
        }
    }

    private string _taskNote;
    public string TaskNote
    {
        get => _taskNote;
        set
        {
            _taskNote = value;
            TaskHelpers.TaskNoteChanged();
        }
    }

    private bool _isChecked;
    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            if (value != _isChecked)
            {
                _isChecked = value;
                TaskHelpers.TaskAlertChanged();
            }
        }
    }
    #endregion Properties
}
