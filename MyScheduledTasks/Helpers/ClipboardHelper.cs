// Copyright (c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

using System.Windows.Threading;

namespace MyScheduledTasks.Helpers;

internal static class ClipboardHelper
{
    #region Copy text to clipboard
    /// <summary>
    /// Copy to clipboard with retry logic to handle potential exceptions when the clipboard is busy.
    /// <param name="text">The text to be copied to the clipboard.</param>
    /// <param name="maxRetries"> is the maximum number of retry attempts.</param>
    /// <param name="delayMs"> is the delay between retries in milliseconds.</param>
    /// <returns>True if the text was successfully copied to the clipboard; otherwise, false.</returns>
    /// </summary>
    public static async System.Threading.Tasks.Task<bool> CopyTextToClipboardAsync(string? text, int maxRetries = 10, int delayMs = 50)
    {
        if (string.IsNullOrEmpty(text) || maxRetries <= 0 || delayMs <= 0)
        {
            return false;
        }

        Dispatcher? dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            return false;
        }

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (dispatcher.CheckAccess())
                {
                    Clipboard.SetText(text);
                }
                else
                {
                    await dispatcher.InvokeAsync(() => Clipboard.SetText(text));
                }
                return true;
            }
            catch (ExternalException) when (attempt < maxRetries)
            {
                await System.Threading.Tasks.Task.Delay(delayMs).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _log.Error(ex, $"Error copying text to clipboard. {ex.Message}");
                return false;
            }
        }
        return false;
    }
    #endregion Copy text to clipboard
}
