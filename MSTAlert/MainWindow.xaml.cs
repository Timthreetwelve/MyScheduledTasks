// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region Using Directives
using System;
using System.Diagnostics;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MyScheduledTasks;
using NLog;
using TKUtils;
#endregion Using Directives

namespace MSTAlert
{
    public partial class MainWindow : Window
    {
        #region NLog
        private static readonly Logger logTemp = LogManager.GetLogger("logTemp");
        #endregion NLog

        public MainWindow()
        {
            UserSettings.Init(UserSettings.AppFolder, UserSettings.DefaultFilename, true);
            InitializeComponent();
            Setup();
            CloseTimer();
        }

        #region Setup
        private void Setup()
        {
            // Change the log file filename when debugging
            if (Debugger.IsAttached)
            {
                GlobalDiagnosticsContext.Set("TempOrDebug", "debug");
            }
            else
            {
                GlobalDiagnosticsContext.Set("TempOrDebug", "temp");
            }

            logTemp.Info($"{AppInfo.AppName} version {AppInfo.TitleVersion} is starting up with argument" +
                         $" \"{GetMessageText()}\"");
            tbMessage.Text = GetMessageText();
            tbTimeStamp.Text = DateTime.Now.ToString("d/M/yyyy • h:mm tt");
        }
        #endregion Setup

        #region Move Window to Bottom Right Corner
        private void PositionWindow()
        {
            var workarea = SystemParameters.WorkArea;
            Left = workarea.Right - Width - 5;
            Top = workarea.Bottom - Height - 4;
        }
        #endregion Move Window to Bottom Right Corner

        #region Process command line to get message
        private static string GetMessageText()
        {
            // If count is less that two, bail out
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length < 2)
            {
                logTemp.Error("Argument count < 2.");
                return "Placeholder Text";
            }
            if (UserSettings.Setting.Sound)
            {
                SystemSounds.Exclamation.Play();
            }
            return args[1].Replace("/", "");
        }
        #endregion Process command line to get message

        #region Move the MessageBox window
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        #endregion Move the MessageBox window

        #region Button Events
        private void Show_Click(object sender, RoutedEventArgs e)
        {
            logTemp.Info($"{AppInfo.AppName} is starting MyScheduledTasks.exe.");
            Process pt = new Process();
            pt.StartInfo.FileName = @".\MyScheduledTasks.exe";
            try
            {
                _ = pt.Start();
            }
            catch (Exception ex)
            {
                logTemp.Error(ex, "Unable to start MyScheduledTasks.exe.");
            }
            Close();
        }
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion Button Events

        #region Window Events
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            logTemp.Info($"{AppInfo.AppName} is shutting down.");
            LogManager.Shutdown();
        }
        #endregion Window Events

        #region Timer Related Methods
        private void CloseTimer()
        {
            if (UserSettings.Setting.AlertTimer > 0)
            {
                StartCloseTimer(UserSettings.Setting.AlertTimer);
                logTemp.Info($"{AppInfo.AppName} will close after {UserSettings.Setting.AlertTimer} minutes.");
            }
        }

        private void StartCloseTimer(double minutes)
        {
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(minutes)
            };
            timer.Tick += TimerElapsed;
            timer.Start();
        }

        private void TimerElapsed(object sender, EventArgs e)
        {
            Debug.WriteLine("In TimerElapsed");
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerElapsed;
            logTemp.Info($"{AppInfo.AppName} timer has expired.");
            Close();
        }
        #endregion Timer Related Methods

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Positioning the window after it has been loaded gives a consistent distance above the taskbar
            PositionWindow();
            Debug.WriteLine($"Actual Height {ActualHeight}");
        }
    }
}
