// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Threading.Tasks;

namespace MyScheduledTasks.ViewModels;

public partial class AboutViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isNewReleaseAvailable;

    [ObservableProperty]
    private bool _releaseCheckDone;

    #region Constructor
    public AboutViewModel()
    {
        if (AnnotatedLanguageList.Count == 0)
        {
            AddNote();
        }
        _ = CheckForNewReleaseOnLoadAsync();
    }
    #endregion Constructor

    private static async Task<bool> CheckForNewReleaseOnLoadAsync()
    {
        if (TempSettings.Setting!.CheckedForNewRelease && UserSettings.Setting!.AutoCheckForUpdates)
        {
            return true;
        }
        TempSettings.Setting.CheckedForNewRelease = true;
        TempSettings.Setting.NewReleaseAvailable = await GitHubHelpers.CheckForNewReleaseAsync();
        TempSettings.Setting.GitHubRelease = GitHubHelpers.GitHubVersion?.ToString() ?? string.Empty;
        Debug.WriteLine($"IsNewReleaseAvailable: {TempSettings.Setting.NewReleaseAvailable}, NewReleaseVersion: {TempSettings.Setting.GitHubRelease}"); // For testing
        return true;
    }

    #region Relay Commands
    [RelayCommand]
    private static void ViewLicense()
    {
        string dir = AppInfo.AppDirectory;
        TextFileViewer.ViewTextFile(Path.Combine(dir, "License.txt"));
    }

    [RelayCommand]
    private static void ViewReadMe()
    {
        string dir = AppInfo.AppDirectory;
        TextFileViewer.ViewTextFile(Path.Combine(dir, "ReadMe.txt"));
    }

    [RelayCommand]
    private static void GoToWebPage(string url)
    {
        Process p = new();
        p.StartInfo.FileName = url;
        p.StartInfo.UseShellExecute = true;
        p.Start();
    }

    [RelayCommand]
    private static async System.Threading.Tasks.Task CheckReleaseAsync()
    {
        await GitHubHelpers.CheckRelease();
    }
    #endregion Relay Commands

    #region Annotated Language translation list
    public List<UILanguage> AnnotatedLanguageList { get; } = [];
    #endregion Annotated Language translation list

    #region Add note to list of languages
    private void AddNote()
    {
        foreach (UILanguage item in UILanguage.DefinedLanguages.Where(item => item.LanguageCode is not "en-GB"))
        {
            item.Note = GetLanguagePercent(item.LanguageCode!);
            AnnotatedLanguageList.Add(item);
        }
    }
    #endregion Add note to list of languages
}
