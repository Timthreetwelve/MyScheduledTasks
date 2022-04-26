// Copyright(c) Tim Kennedy. All Rights Reserved. Licensed under the MIT License.

namespace MyScheduledTasks;

internal static class NativeMethods
{
    #region Window button voodoo
    /// <summary>
    /// Method to remove the minimize and maximize/restore buttons
    /// </summary>
    /// <param name="windowHandle"></param>
    internal static void DisableMinMaxButtons(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }
        _ = SetWindowLong(windowHandle,
                          GWL_STYLE,
                          GetWindowLong(windowHandle, GWL_STYLE) & ~WS_BOTHBOXES);
    }

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    private const int GWL_STYLE = -16;
    private const int WS_MAXIMIZEBOX = 0x10000;                        // maximize button
    private const int WS_MINIMIZEBOX = 0x20000;                        // minimize button
    private const int WS_BOTHBOXES = WS_MINIMIZEBOX + WS_MAXIMIZEBOX;  // Both
    #endregion Window button voodoo

    #region For single instance app
    [DllImport("User32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern Boolean IsIconic([In] IntPtr windowHandle);

    [DllImport("User32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern Boolean SetForegroundWindow([In] IntPtr windowHandle);

    [DllImport("User32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern Boolean ShowWindow([In] IntPtr windowHandle, [In] ShowWindowCommand command);

    public enum ShowWindowCommand : int
    {
        Hide = 0x0,
        ShowNormal = 0x1,
        ShowMinimized = 0x2,
        ShowMaximized = 0x3,
        ShowNormalNotActive = 0x4,
        Minimize = 0x6,
        ShowMinimizedNotActive = 0x7,
        ShowCurrentNotActive = 0x8,
        Restore = 0x9,
        ShowDefault = 0xA,
        ForceMinimize = 0xB
    }
    #endregion For single instance app
}
