using System.Runtime.InteropServices;

namespace TidyTop.App.Services;

/// <summary>
/// Hides or shows Windows' native desktop icon list view without touching the files on disk.
/// This is the safe first step toward a Fences-style managed desktop mode.
/// </summary>
public sealed class WindowsNativeDesktopIconService : INativeDesktopIconService
{
    private bool? _capturedVisibility;

    public bool IsSupported => OperatingSystem.IsWindows();

    public void CaptureInitialState()
    {
        if (!IsSupported || _capturedVisibility.HasValue)
        {
            return;
        }

        _capturedVisibility = AreIconsVisible();
    }

    public bool AreIconsVisible()
    {
        if (!IsSupported)
        {
            return true;
        }

        var desktopIconView = DesktopIconWindowFinder.FindDesktopIconView();
        return desktopIconView == IntPtr.Zero || NativeMethods.IsWindowVisible(desktopIconView);
    }

    public void SetIconsVisible(bool visible)
    {
        if (!IsSupported)
        {
            return;
        }

        var desktopIconView = DesktopIconWindowFinder.FindDesktopIconView();
        if (desktopIconView == IntPtr.Zero)
        {
            return;
        }

        NativeMethods.ShowWindow(desktopIconView, visible ? NativeMethods.ShowWindowCommand.Show : NativeMethods.ShowWindowCommand.Hide);
    }

    public void RestoreCapturedState()
    {
        if (!_capturedVisibility.HasValue)
        {
            return;
        }

        SetIconsVisible(_capturedVisibility.Value);
    }

    private static class DesktopIconWindowFinder
    {
        public static IntPtr FindDesktopIconView()
        {
            var progman = NativeMethods.FindWindow("Progman", null);
            var shellView = FindShellView(progman);
            if (shellView != IntPtr.Zero)
            {
                return shellView;
            }

            var found = IntPtr.Zero;
            NativeMethods.EnumWindows((topLevelWindow, _) =>
            {
                shellView = FindShellView(topLevelWindow);
                if (shellView == IntPtr.Zero)
                {
                    return true;
                }

                found = shellView;
                return false;
            }, IntPtr.Zero);

            return found;
        }

        private static IntPtr FindShellView(IntPtr parent)
        {
            if (parent == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            var shellView = NativeMethods.FindWindowEx(parent, IntPtr.Zero, "SHELLDLL_DefView", null);
            return shellView;
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
    }
}
