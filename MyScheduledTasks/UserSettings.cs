// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using directives
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TKUtils;
#endregion Using directives

namespace MyScheduledTasks
{
    public class UserSettings : SettingsManager<UserSettings>, INotifyPropertyChanged
    {
        #region Constructor
        public UserSettings()
        {
            // Set defaults
            AddWindowHeight = 450;
            AddWindowWidth = 800;
            GridZoom = 1;
            HideMicrosoftFolder = true;
            SaveOnExit = true;
            ShadeAltRows = true;
            ShowAlertCol = true;
            ShowFolderCol = true;
            ShowLastRunCol = true;
            ShowNoteCol = true;
            ShowResultCol = true;
            ShowStatusCol = true;
            SuppressFileSaveNotify = true;
            WindowLeft = 100;
            WindowTop = 100;
        }
        #endregion Constructor

        #region Properties

        public double AddWindowHeight { get; set; }

        public double AddWindowWidth { get; set; }

        public double GridZoom
        {
            get
            {
                if (gridZoom <= 0)
                {
                    gridZoom = 1;
                }
                return gridZoom;
            }
            set
            {
                gridZoom = value;
                OnPropertyChanged();
            }
        }

        public bool HideMicrosoftFolder
        {
            get { return hideMicrosoftFolder; }
            set
            {
                hideMicrosoftFolder = value;
                OnPropertyChanged();
            }
        }

        public bool SaveOnExit
        {
            get { return saveOnExit; }
            set
            {
                saveOnExit = value;
                OnPropertyChanged();
            }
        }

        public bool ShadeAltRows
        {
            get => shadeAltRows;
            set
            {
                shadeAltRows = value;
                OnPropertyChanged();
            }
        }

        public bool ShowAlertCol
        {
            get { return showAlertCol; }
            set
            {
                showAlertCol = value;
                OnPropertyChanged();
            }
        }

        public bool ShowFolderCol
        {
            get { return showFolderCol; }
            set
            {
                showFolderCol = value;
                OnPropertyChanged();
            }
        }

        public bool ShowLastRunCol
        {
            get { return showLastRunCol; }
            set
            {
                showLastRunCol = value;
                OnPropertyChanged();
            }
        }

        public bool ShowNoteCol
        {
            get { return showNoteCol; }
            set
            {
                showNoteCol = value;
                OnPropertyChanged();
            }
        }

        public bool ShowResultCol
        {
            get { return showResultCol; }
            set
            {
                showResultCol = value;
                OnPropertyChanged();
            }
        }

        public bool ShowStatusCol
        {
            get { return showStatusCol; }
            set
            {
                showStatusCol = value;
                OnPropertyChanged();
            }
        }

        public bool SuppressFileSaveNotify
        {
            get { return suppressFileSaveNotify; }
            set
            {
                suppressFileSaveNotify = value;
                OnPropertyChanged();
            }
        }

        public double WindowLeft
        {
            get
            {
                if (windowLeft < 0)
                {
                    windowLeft = 100;
                }
                return windowLeft;
            }
            set => windowLeft = value;
        }

        public double WindowTop
        {
            get
            {
                if (windowTop < 0)
                {
                    windowTop = 100;
                }
                return windowTop;
            }
            set => windowTop = value;
        }
        #endregion Properties

        #region Private backing fields
        private bool hideMicrosoftFolder;
        private bool saveOnExit;
        private bool shadeAltRows;
        private bool showAlertCol;
        private bool showFolderCol;
        private bool showLastRunCol;
        private bool showNoteCol;
        private bool showResultCol;
        private bool showStatusCol;
        private bool suppressFileSaveNotify;
        private double gridZoom;
        private double windowLeft;
        private double windowTop;
        #endregion Private backing fields

        #region Handle property change event
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion Handle property change event
    }
}
