using NLog;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using TKUtils;

namespace MyScheduledTasks
{
    /// <summary>
    /// Interaction logic for ShowSettings.xaml
    /// </summary>
    public partial class ShowSettings : Window
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public ShowSettings()
        {
            InitializeComponent();

            Refresh();
        }

        private void Refresh()
        {
            List<MySettings> settings = ReadSettings();

            GetSystemParameters(settings);

            dg1.ItemsSource = settings.OrderBy(x => x.Name);
        }

        private static void GetSystemParameters(List<MySettings> settings)
        {
            MySettings sp1 = new MySettings
            {
                Name = "Application Path",
                Value = AppInfo.AppPath
            };
            settings.Add(sp1);

            MySettings sp2 = new MySettings
            {
                Name = "Primary Screen Height",
                Value = $"{SystemParameters.PrimaryScreenHeight}"
            };
            settings.Add(sp2);

            MySettings sp3 = new MySettings
            {
                Name = "Primary Screen Width",
                Value = $"{SystemParameters.PrimaryScreenWidth}"
            };
            settings.Add(sp3);
        }

        private static List<MySettings> ReadSettings()
        {
            List<MySettings> settings = new List<MySettings>();
            foreach (SettingsPropertyValue property in Properties.Settings.Default.PropertyValues)
            {
                MySettings show = new MySettings();
                show.Value = property.PropertyValue.ToString();
                show.Name = property.Name;
                settings.Add(show);
            }

            return settings;
        }

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            string fullName = AppInfo.AppPath;
            Clipboard.SetText(fullName);
            log.Debug($"{fullName} copied to clipboard");
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F5)
            {
                Refresh();
            }

            if (e.Key == System.Windows.Input.Key.Escape)
            {
                Close();
            }
        }
    }
}
