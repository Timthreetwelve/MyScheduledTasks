// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

#region using directives
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Navigation;
using TKUtils;
#endregion

namespace MyScheduledTasks
{
    public partial class About : Window
    {
        public About()
        {
            InitializeComponent();
        }

        #region Mouse events
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
        #endregion

        #region Window events
        private void Window_Activated(object sender, System.EventArgs e)
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

            string version = versionInfo.FileVersion;
            string copyright = versionInfo.LegalCopyright;
            string product = versionInfo.ProductName;

            tbVersion.Text = version.Remove(version.LastIndexOf("."));
            tbCopyright.Text = copyright.Replace("Copyright ", "");
            Title = $"About {product}";
            Topmost = true;
        }
        #endregion

        #region Link events
        private void ReadMeLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Topmost = false;
                using (Process ps = new Process())
                {
                    ps.StartInfo.FileName = ".\\ReadMe.txt";
                    ps.StartInfo.UseShellExecute = true;
                    _ = ps.Start();
                }
                e.Handled = true;
                Close();
            }
            catch (System.Exception ex)
            {
                _ = TKMessageBox.Show($"Error opening ReadMe.txt\n{ex}",
                                      "PathTools Error",
                                      MessageBoxButton.OK,
                                      TKUtils.MessageBoxImage.Error);
            }
        }

        private void OnNavigate(object sender, RequestNavigateEventArgs e)
        {
            Debug.WriteLine($"Opening {e.Uri.AbsoluteUri}");
            try
            {
                using (Process ps = new Process())
                {
                    ps.StartInfo.FileName = e.Uri.AbsoluteUri;
                    ps.StartInfo.UseShellExecute = true;
                    _ = ps.Start();
                }
                e.Handled = true;
                Close();
            }
            catch (System.Exception ex)
            {
                _ = TKMessageBox.Show($"Error opening the link\n{ex}",
                                      "PathTools Error",
                                      MessageBoxButton.OK,
                                      TKUtils.MessageBoxImage.Error);
            }
        }
        #endregion Link events
    }
}
