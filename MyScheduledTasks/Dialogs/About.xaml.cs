// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks.Dialogs;

/// <summary>
/// Dialog to display the About information and handle navigation
/// </summary>
public partial class About : UserControl
{
    public About()
    {
        InitializeComponent();

        txtBuildDate.Text = $"{BuildInfo.BuildDateUtc:f}  (UTC)";
    }

    #region License click
    private void BtnLicense_Click(object sender, RoutedEventArgs e)
    {
        string dir = AppInfo.AppDirectory;
        TextFileViewer.ViewTextFile(Path.Combine(dir, "License.txt"));
    }
    #endregion License click

    #region URL click
    private void OnNavigate(object sender, RequestNavigateEventArgs e)
    {
        Process p = new();
        p.StartInfo.FileName = e.Uri.AbsoluteUri;
        p.StartInfo.UseShellExecute = true;
        p.Start();
        e.Handled = true;
    }
    #endregion URL click
}
