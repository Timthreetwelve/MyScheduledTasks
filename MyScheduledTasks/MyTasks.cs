// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks
{
    public class MyTasks : INotifyPropertyChanged
    {
        #region Constructor
        public MyTasks(string taskPath, bool alert, string taskNote)
        {
            TaskPath = taskPath;
            Alert = alert;
            TaskNote = taskNote;
        }
        #endregion

        #region Private backing fields
        private string taskPath;
        private bool alert;
        private string taskNote;
        #endregion

        #region Properties
        public string TaskPath
        {
            get { return taskPath; }
            set
            {
                if (value != null)
                {
                    taskPath = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool Alert
        {
            get => alert;
            set
            {
                alert = value;
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

        public static bool IsDirty { get; set; }

        public static bool SortIsDirty { get; set; }

        #endregion

        #region Observable collection
        public static ObservableCollection<MyTasks> MyTasksCollection { get; set; }
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
