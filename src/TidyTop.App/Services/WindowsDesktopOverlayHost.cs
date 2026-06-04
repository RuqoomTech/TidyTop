using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Platform;

namespace TidyTop.App.Services;

/// <summary>
/// Windows implementation of the desktop-hosting trick used by many desktop widget/overlay apps.
/// It parents TidyTop to the hidden WorkerW/Progman desktop window so SmartBoxes live on the desktop
/// rather than inside a normal app window.
/// </summary>
public sealed class WindowsDesktopOverlayHost : IDesktopOverlayHost
{
    public void AttachToDesktop(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var platformHandle = window.TryGetPlatformHandle();
        if (platformHandle is null || platformHandle.Handle == IntPtr.Zero)
        {
            return;
        }

        var appWindowHandle = platformHandle.Handle;
        ConfigureAppWindowStyles(appWindowHandle);

        var desktopHostHandle = DesktopWindowFinder.FindDesktopHostWindow();
        if (desktopHostHandle == IntPtr.Zero)
        {
            return;
        }

        NativeMethods.SetParent(appWindowHandle, desktopHostHandle);
        NativeMethods.SetWindowPos(
            appWindowHandle,
            IntPtr.Zero,
            0,
            0,
            0,
            0,
            NativeMethods.SetWindowPosFlags.NoMove
            | NativeMethods.SetWindowPosFlags.NoSize
            | NativeMethods.SetWindowPosFlags.NoZOrder
            | NativeMethods.SetWindowPosFlags.FrameChanged
            | NativeMethods.SetWindowPosFlags.ShowWindow);
    }

    private static void ConfigureAppWindowStyles(IntPtr windowHandle)
    {
        var extendedStyle = NativeMethods.GetWindowLongPtr(windowHandle, NativeMethods.WindowLongIndex.ExtendedStyle).ToInt64();
        extendedStyle |= NativeMethods.WindowStylesExtended.ToolWindow;
        extendedStyle &= ~NativeMethods.WindowStylesExtended.AppWindow;
        NativeMethods.SetWindowLongPtr(windowHandle, NativeMethods.WindowLongIndex.ExtendedStyle, new IntPtr(extendedStyle));
    }

    private static class DesktopWindowFinder
    {
        private const uint CreateWorkerWMessage = 0x052C;

        public static IntPtr FindDesktopHostWindow()
        {
            var progman = NativeMethods.FindWindow("Progman", null);
            if (progman == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            // Ask Explorer to create the additional WorkerW surface that sits behind the desktop icon list.
            NativeMethods.SendMessageTimeout(
                progman,
                CreateWorkerWMessage,
                UIntPtr.Zero,
                IntPtr.Zero,
                NativeMethods.SendMessageTimeoutFlags.Normal,
                1000,
                out _);

            var workerW = IntPtr.Zero;
            NativeMethods.EnumWindows((topLevelWindow, _) =>
            {
                var shellView = NativeMethods.FindWindowEx(topLevelWindow, IntPtr.Zero, "SHELLDLL_DefView", null);
                if (shellView == IntPtr.Zero)
                {
                    return true;
                }

                workerW = NativeMethods.FindWindowEx(IntPtr.Zero, topLevelWindow, "WorkerW", null);
                return false;
            }, IntPtr.Zero);

            return workerW != IntPtr.Zero ? workerW : progman;
        }
    }

    private static class NativeMethods
    {
        public static class WindowLongIndex
        {
            public const int ExtendedStyle = -20;
        }

        public static class WindowStylesExtended
        {
            public const long AppWindow = 0x00040000L;
            public const long ToolWindow = 0x00000080L;
        }

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            Normal = 0x0000,
            Block = 0x0001,
            AbortIfHung = 0x0002,
            NoTimeoutIfNotHung = 0x0008
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            NoSize = 0x0001,
            NoMove = 0x0002,
            NoZOrder = 0x0004,
            NoActivate = 0x0010,
            FrameChanged = 0x0020,
            ShowWindow = 0x0040
        }

        public delegate bool EnumWindowsProc(IntPtr windowHandle, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "FindWindowW", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string className, string? windowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowExW", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfterHandle, string? className, string? windowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumWindowsProc enumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr childWindowHandle, IntPtr newParentWindowHandle);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            IntPtr windowHandle,
            IntPtr insertAfterHandle,
            int x,
            int y,
            int width,
            int height,
            SetWindowPosFlags flags);

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeoutW", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint message,
            UIntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags flags,
            uint timeoutMilliseconds,
            out UIntPtr result);

        public static IntPtr GetWindowLongPtr(IntPtr windowHandle, int index)
        {
            return IntPtr.Size == 8
                ? GetWindowLongPtr64(windowHandle, index)
                : new IntPtr(GetWindowLong32(windowHandle, index));
        }

        public static IntPtr SetWindowLongPtr(IntPtr windowHandle, int index, IntPtr value)
        {
            return IntPtr.Size == 8
                ? SetWindowLongPtr64(windowHandle, index, value)
                : new IntPtr(SetWindowLong32(windowHandle, index, value.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLongW", SetLastError = true)]
        private static extern int GetWindowLong32(IntPtr windowHandle, int index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongW", SetLastError = true)]
        private static extern int SetWindowLong32(IntPtr windowHandle, int index, int value);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtrW", SetLastError = true)]
        private static extern IntPtr GetWindowLongPtr64(IntPtr windowHandle, int index);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW", SetLastError = true)]
        private static extern IntPtr SetWindowLongPtr64(IntPtr windowHandle, int index, IntPtr value);
    }
}
