﻿// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

// Leave the Octokit using statement here. It's a problem in GlobalUsings.cs
using Octokit;
using System.Threading.Tasks;

namespace MyScheduledTasks.Helpers;

/// <summary>
/// Class for methods that check GitHub for releases
/// </summary>
internal static class GitHubHelpers
{
    #region MainWindow Instance
    private static readonly MainWindow? _mainWindow = System.Windows.Application.Current.MainWindow as MainWindow;
    #endregion MainWindow Instance

    /// <summary>
    /// The application version from GitHub.
    /// </summary>
    public static Version? GitHubVersion { get; private set; }

    #region Check for newer release
    /// <summary>
    /// Checks to see if a newer release is available.
    /// </summary>
    /// <remarks>
    /// If the release version is greater than the current version
    /// a message box will be shown asking to go to the releases page.
    /// </remarks>
    public static async System.Threading.Tasks.Task CheckRelease()
    {
        try
        {
            SnackbarMsg.ClearAndQueueMessage(GetStringResource("MsgText_AppUpdateChecking"));
            Release release = await GetLatestReleaseAsync(AppConstString.RepoOwner, AppConstString.RepoName);
            if (release.TagName == null)
            {
                CheckFailed();
                return;
            }
            string tag = release.TagName;

            if (tag.StartsWith("v", StringComparison.InvariantCultureIgnoreCase))
            {
                tag = tag.ToLower(CultureInfo.InvariantCulture).TrimStart('v');
            }

            GitHubVersion = new(tag);

            _log.Debug($"Latest version is {GitHubVersion} released on {release.PublishedAt!.Value.UtcDateTime} UTC");

            if (GitHubVersion <= AppInfo.AppVersionVer)
            {
                string msg = GetStringResource("MsgText_AppUpdateNoneFound");
                _log.Debug("No newer releases were found.");
                _ = new MDCustMsgBox(msg,
                    "My Scheduled Tasks",
                    ButtonType.Ok,
                    false,
                    true,
                    _mainWindow).ShowDialog();
            }
            else
            {
                _log.Debug($"A newer release ({GitHubVersion}) has been found.");
                string msg = string.Format(CultureInfo.InvariantCulture, MsgTextAppUpdateNewerFound, GitHubVersion);
                _ = new MDCustMsgBox($"{msg}\n\n" +
                                            $"{GetStringResource("MsgText_AppUpdateGoToRelease")}\n\n" +
                                            $"{GetStringResource("MsgText_AppUpdateCloseApp")}",
                    "My Scheduled Tasks",
                    ButtonType.YesNo,
                    false,
                    true,
                    _mainWindow).ShowDialog();

                if (MDCustMsgBox.CustResult == CustResultType.Yes)
                {
                    _log.Debug($"Opening {release.HtmlUrl}");
                    string url = release.HtmlUrl;
                    Process p = new();
                    p.StartInfo.FileName = url;
                    p.StartInfo.UseShellExecute = true;
                    p.Start();
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error encountered while checking version");
            CheckFailed();
        }
    }
    #endregion Check for newer release

    #region Check for new release async
    /// <summary>
    /// Checks to see if a newer release is available asynchronously.
    /// </summary>
    /// <returns>True if a newer release is available, otherwise false.</returns>
    public static async Task<bool> CheckForNewReleaseAsync()
    {
        try
        {
            Release release = await GetLatestReleaseAsync(AppConstString.RepoOwner, AppConstString.RepoName);
            if (release.TagName == null)
            {
                return false;
            }

            string tag = release.TagName.Trim();
            if (string.IsNullOrEmpty(tag))
            {
                return false;
            }

            if (tag.StartsWith("v", StringComparison.InvariantCultureIgnoreCase))
            {
                tag = tag[1..]; // Remove the leading 'v'
            }

            if (!Version.TryParse(tag, out var version))
            {
                _log.Error($"Failed to parse version tag: {tag}");
                return false;
            }

            GitHubVersion = version;
            _log.Debug($"Latest version is {GitHubVersion} released on {release.PublishedAt!.Value.UtcDateTime} UTC");
            return GitHubVersion > AppInfo.AppVersionVer;
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Error encountered while checking GitHub for latest release.");
            return false;
        }
    }
    #endregion Check for new release async

    #region Get latest release
    /// <summary>
    /// Gets the latest release.
    /// </summary>
    /// <param name="repoOwner">The repository owner.</param>
    /// <param name="repoName">Name of the repository.</param>
    /// <returns>Release object</returns>
    private static async Task<Release> GetLatestReleaseAsync(string repoOwner, string repoName)
    {
        try
        {
            GitHubClient client = new(new ProductHeaderValue(repoName));
            _log.Debug("Checking GitHub for latest release.");
            return await client.Repository.Release.GetLatest(repoOwner, repoName);
        }
        catch (Exception ex)
        {
            _log.Error(ex, "Get latest release from GitHub failed.");
            return new();
        }
    }
    #endregion Get latest release

    #region Check failed message
    /// <summary>
    /// Display a message box stating that the release check failed.
    /// </summary>
    private static void CheckFailed()
    {
        _ = new MDCustMsgBox(GetStringResource("MsgText_AppUpdateCheckFailed"),
            "My Scheduled Tasks",
            ButtonType.Ok,
            false,
            true,
            _mainWindow,
            true).ShowDialog();
    }
    #endregion Check failed message
}
