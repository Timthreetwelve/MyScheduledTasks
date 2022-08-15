// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks;

public class UserSettings : SettingsManager<UserSettings>, INotifyPropertyChanged
{
    #region Methods
    public void SaveWindowPos()
    {
        Window mainWindow = Application.Current.MainWindow;
        WindowHeight = Math.Floor(mainWindow.Height);
        WindowLeft = Math.Floor(mainWindow.Left);
        WindowTop = Math.Floor(mainWindow.Top);
        WindowWidth = Math.Floor(mainWindow.Width);
    }

    public void SetWindowPos()
    {
        Window mainWindow = Application.Current.MainWindow;
        mainWindow.Height = WindowHeight;
        mainWindow.Left = WindowLeft;
        mainWindow.Top = WindowTop;
        mainWindow.Width = WindowWidth;
    }
    #endregion Methods

    #region Properties
    public int DarkMode
    {
        get => darkmode;
        set
        {
            darkmode = value;
            OnPropertyChanged();
        }
    }

    public double DetailsHeight
    {
        get { return detailsHeight; }
        set
        {
            detailsHeight = value;
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

    public bool IncludeDebug
    {
        get => includeDebug;
        set
        {
            includeDebug = value;
            OnPropertyChanged();
        }
    }

    public bool KeepOnTop
    {
        get => keepOnTop;
        set
        {
            keepOnTop = value;
            OnPropertyChanged();
        }
    }

    public bool NewLog
    {
        get => newLog;
        set
        {
            newLog = value;
            OnPropertyChanged();
        }
    }

    public int PrimaryColor
    {
        get => primaryColor;
        set
        {
            primaryColor = value;
            OnPropertyChanged();
        }
    }

    public int RowSpacing
    {
        get => rowSpacing;
        set
        {
            rowSpacing = value;
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

    public bool ShowAlertCol
    {
        get { return showAlertCol; }
        set
        {
            showAlertCol = value;
            OnPropertyChanged();
        }
    }

    public bool ShowDetails
    {
        get { return showDetails; }
        set
        {
            showDetails = value;
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

    public bool Sound
    {
        get { return sound; }
        set
        {
            sound = value;
            OnPropertyChanged();
        }
    }

    public int UISize
    {
        get => uiSize;
        set
        {
            uiSize = value;
            OnPropertyChanged();
        }
    }

    public double WindowHeight
    {
        get
        {
            if (windowHeight < 100)
            {
                windowHeight = 100;
            }
            return windowHeight;
        }
        set => windowHeight = value;
    }

    public double WindowLeft
    {
        get
        {
            if (windowLeft < 0 || windowLeft >= SystemParameters.VirtualScreenWidth)
            {
                windowLeft = 0;
            }
            return windowLeft;
        }
        set => windowLeft = value;
    }

    public double WindowTop
    {
        get
        {
            if (windowTop < 0 || windowTop >= SystemParameters.VirtualScreenHeight)
            {
                windowTop = 0;
            }
            return windowTop;
        }
        set => windowTop = value;
    }

    public double WindowWidth
    {
        get
        {
            if (windowWidth < 100)
            {
                windowWidth = 100;
            }
            return windowWidth;
        }
        set => windowWidth = value;
    }
    #endregion Properties

    #region Private backing fields
    private int darkmode = (int)ThemeType.Light;
    private double detailsHeight = 300;
    private bool hideMicrosoftFolder = true;
    private bool includeDebug = true;
    private bool keepOnTop;
    private bool newLog = true;
    private int primaryColor = (int)AccentColor.Blue;
    private int rowSpacing = (int)Spacing.Comfortable;
    private bool saveOnExit = true;
    private bool showAlertCol = true;
    private bool showDetails = true;
    private bool showFolderCol = true;
    private bool showLastRunCol = true;
    private bool showNoteCol = true;
    private bool showResultCol = true;
    private bool showStatusCol = true;
    private bool sound = true;
    private int uiSize = (int)MySize.Default;
    private double windowHeight = 500;
    private double windowLeft = 100;
    private double windowTop = 100;
    private double windowWidth = 700;
    #endregion Private backing fields

    #region Handle property change event
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion Handle property change event
}
