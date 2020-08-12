// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using NLog;
using System;
using System.Diagnostics;
using System.Windows;

namespace MyScheduledTasks
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            OneInstance();

            base.OnStartup(e);
        }

        #region Only One Instance
        private static void OneInstance()
        {
            // Ensure only one instance of the process running
            Process currentProcess = Process.GetCurrentProcess();

            foreach (var AllProcesses in Process.GetProcesses())
            {
                if (AllProcesses.Id != currentProcess.Id && AllProcesses.ProcessName == currentProcess.ProcessName)
                {
                    log.Error($"This is  {currentProcess.ProcessName} {currentProcess.Id}. " +
                              $"- {AllProcesses.ProcessName} {AllProcesses.Id} is also running.");
                    log.Error($"Another instance of {currentProcess.ProcessName} is already running!  Shutting this one down.");

                    _ = MessageBox.Show($"An instance of {currentProcess.ProcessName} is already running",
                                        currentProcess.ProcessName,
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Exclamation,
                                        MessageBoxResult.OK,
                                        MessageBoxOptions.DefaultDesktopOnly);
                    Environment.Exit(1);
                    break;
                }
            }
        }
        #endregion Only One Instance
    }
}