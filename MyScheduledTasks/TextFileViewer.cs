// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks;

internal static class TextFileViewer
{
    #region NLog Instance
    private static readonly Logger logTemp = LogManager.GetLogger("logTemp");
    #endregion NLog Instance

    #region Text file viewer
    /// <summary>
    /// Method for viewing text files in the default app
    /// </summary>
    /// <param name="txtfile">Full path to the text file</param>
    public static void ViewTextFile(string txtfile)
    {
        if (File.Exists(txtfile))
        {
            try
            {
                using Process p = new();
                p.StartInfo.FileName = txtfile;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.ErrorDialog = false;
                _ = p.Start();
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1155)
                {
                    using Process p = new();
                    p.StartInfo.FileName = "notepad.exe";
                    p.StartInfo.Arguments = txtfile;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.ErrorDialog = false;
                    _ = p.Start();
                }
                else
                {
                    logTemp.Error(ex, $"Unable to open {txtfile}");
                    string msg = $"Unable to open {txtfile}. See the log file for more information.";
                    DisplayDialog(msg);
                }
            }
            catch (Exception ex)
            {
                logTemp.Error(ex, $"Unable to open {txtfile}");
                string msg = $"Unable to open {txtfile}. See the log file for more information.";
                DisplayDialog(msg);
            }
        }
        else
        {
            logTemp.Error($"File not found {txtfile}");
            string msg = $"File not found {txtfile}.";
            DisplayDialog(msg);
        }
    }
    #endregion Text file viewer

    #region Display an error dialog
    private static async void DisplayDialog(string msg)
    {
        if (!DialogHost.IsDialogOpen("MainDialogHost"))
        {
            SystemSounds.Exclamation.Play();
            ErrorDialog error = new()
            {
                Message = msg
            };
            _ = await DialogHost.Show(error, "MainDialogHost");
        }
        else
        {
            DialogHost.Close("MainDialogHost");
            SystemSounds.Exclamation.Play();
            ErrorDialog error = new()
            {
                Message = msg
            };
            _ = await DialogHost.Show(error, "MainDialogHost");
        }
    }
    #endregion Display an error dialog
}
