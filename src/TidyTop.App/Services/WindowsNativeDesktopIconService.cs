using System.Runtime.InteropServices;
using TidyTop.Core.Services;

namespace TidyTop.App.Services;

/// <summary>
/// Hides or shows Windows' native desktop icon list view without touching the files on disk.
/// This service only changes Explorer window visibility. It never moves, deletes, or rewrites files.
/// </summary>
public sealed class WindowsNativeDesktopIconService : INativeDesktopIconService
{
    private readonly IAppLogger _logger;
    private bool? _capturedVisibility;

    public WindowsNativeDesktopIconService(IAppLogger? logger = null)
    {
        _logger = logger ?? NullAppLogger.Instance;
    }

    public bool IsSupported => OperatingSystem.IsWindows();

    public void CaptureInitialState()
    {
        if (!IsSupported || _capturedVisibility.HasValue)
        {
            return;
        }

        _capturedVisibility = AreIconsVisible();
        _logger.Info($"Captured native desktop icon visibility: {_capturedVisibility.Value}.");
    }

    public bool AreIconsVisible()
    {
        if (!IsSupported)
        {
            return true;
        }

        var windows = DesktopIconWindowFinder.FindDesktopIconWindows();
        return windows.IconListView == IntPtr.Zero || NativeMethods.IsWindowVisible(windows.IconListView);
    }

    public void SetIconsVisible(bool visible)
    {
        if (!IsSupported)
        {
            return;
        }

        try
        {
            var windows = DesktopIconWindowFinder.FindDesktopIconWindows();
            if (windows.IsEmpty)
            {
                _logger.Warning("Could not find the Windows desktop icon view. Native icon visibility was not changed.");
                return;
            }

            // Show the parent DefView first, then the actual SysListView32 icon list.
            // Hiding/showing only one of them is unreliable across Explorer versions.
            SetVisible(windows.ShellDefView, visible);
            SetVisible(windows.IconListView, visible);

            Redraw(windows.IconListView);
            Redraw(windows.ShellDefView);
            _logger.Info($"Set native desktop icon visibility to {visible}.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Could not set native desktop icon visibility to {visible}.", ex);
        }
    }

    public void RestoreCapturedState()
    {
        if (!_capturedVisibility.HasValue)
        {
            // Failsafe: if TidyTop exits without a captured state, prefer visible native icons.
            SetIconsVisible(true);
            return;
        }

        SetIconsVisible(_capturedVisibility.Value);
    }

    private static void SetVisible(IntPtr windowHandle, bool visible)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }

        NativeMethods.ShowWindow(windowHandle, visible ? NativeMethods.ShowWindowCommand.Show : NativeMethods.ShowWindowCommand.Hide);
    }

    private static void Redraw(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }

        NativeMethods.InvalidateRect(windowHandle, IntPtr.Zero, true);
        NativeMethods.UpdateWindow(windowHandle);
    }

    private readonly record struct DesktopIconWindows(IntPtr ShellDefView, IntPtr IconListView)
    {
        public bool IsEmpty => ShellDefView == IntPtr.Zero && IconListView == IntPtr.Zero;
    }

    private static class DesktopIconWindowFinder
    {
        public static DesktopIconWindows FindDesktopIconWindows()
        {
            var progman = NativeMethods.FindWindow("Progman", null);
            var windows = FindUnderTopLevelWindow(progman);
            if (!windows.IsEmpty)
            {
                return windows;
            }

            var found = new DesktopIconWindows(IntPtr.Zero, IntPtr.Zero);
            NativeMethods.EnumWindows((topLevelWindow, _) =>
            {
                windows = FindUnderTopLevelWindow(topLevelWindow);
                if (windows.IsEmpty)
                {
                    return true;
                }

                found = windows;
                return false;
            }, IntPtr.Zero);

            return found;
        }

        private static DesktopIconWindows FindUnderTopLevelWindow(IntPtr parent)
        {
            if (parent == IntPtr.Zero)
            {
                return new DesktopIconWindows(IntPtr.Zero, IntPtr.Zero);
            }

            var shellDefView = NativeMethods.FindWindowEx(parent, IntPtr.Zero, "SHELLDLL_DefView", null);
            if (shellDefView == IntPtr.Zero)
            {
                return new DesktopIconWindows(IntPtr.Zero, IntPtr.Zero);
            }

            var iconListView = NativeMethods.FindWindowEx(shellDefView, IntPtr.Zero, "SysListView32", "FolderView");
            if (iconListView == IntPtr.Zero)
            {
                iconListView = NativeMethods.FindWindowEx(shellDefView, IntPtr.Zero, "SysListView32", null);
            }

            return new DesktopIconWindows(shellDefView, iconListView);
        }
    }

    private static class NativeMethods
    {
        public enum ShowWindowCommand
        {
            Hide = 0,
            Show = 5
        }

        public delegate bool EnumWindowsProc(IntPtr windowHandle, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindowW", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string className, string? windowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfterHandle, string? className, string? windowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc enumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr windowHandle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ShowWindow(IntPtr windowHandle, ShowWindowCommand command);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InvalidateRect(IntPtr windowHandle, IntPtr rect, bool erase);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UpdateWindow(IntPtr windowHandle);
    }
}
