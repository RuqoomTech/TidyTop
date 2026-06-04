using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TidyTop.App.Services;

/// <summary>
/// Lightweight global hotkey service for Ctrl+Alt+T. It uses a low-level keyboard hook instead
/// of RegisterHotKey so the first pass does not depend on Avalonia/Win32 message interop.
/// </summary>
public sealed class WindowsGlobalHotkeyService : IGlobalHotkeyService
{
    private const int WhKeyboardLl = 13;
    private const int WmKeyDown = 0x0100;
    private const int WmKeyUp = 0x0101;
    private const int WmSysKeyDown = 0x0104;
    private const int WmSysKeyUp = 0x0105;
    private const int VkControl = 0x11;
    private const int VkMenu = 0x12;
    private const int VkT = 0x54;

    private NativeMethods.LowLevelKeyboardProc? _hookProc;
    private IntPtr _hookHandle;
    private bool _isHotkeyDown;

    public event EventHandler? ToggleRequested;

    public bool IsSupported => OperatingSystem.IsWindows();
    public bool IsRunning => _hookHandle != IntPtr.Zero;

    public void Start()
    {
        if (!IsSupported || IsRunning)
        {
            return;
        }

        _hookProc = HookCallback;
        _hookHandle = NativeMethods.SetWindowsHookEx(WhKeyboardLl, _hookProc, GetCurrentModuleHandle(), 0);
    }

    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }

        NativeMethods.UnhookWindowsHookEx(_hookHandle);
        _hookHandle = IntPtr.Zero;
        _hookProc = null;
        _isHotkeyDown = false;
    }

    public void Dispose()
    {
        Stop();
    }

    private IntPtr HookCallback(int code, IntPtr wParam, IntPtr lParam)
    {
        if (code >= 0)
        {
            var message = wParam.ToInt32();
            var keyboard = Marshal.PtrToStructure<NativeMethods.KeyboardHookStruct>(lParam);
            if (keyboard.VirtualKeyCode == VkT && (message == WmKeyDown || message == WmSysKeyDown))
            {
                var ctrlPressed = IsKeyDown(VkControl);
                var altPressed = IsKeyDown(VkMenu);
                if (ctrlPressed && altPressed && !_isHotkeyDown)
                {
                    _isHotkeyDown = true;
                    ToggleRequested?.Invoke(this, EventArgs.Empty);
                }
            }
            else if (keyboard.VirtualKeyCode == VkT && (message == WmKeyUp || message == WmSysKeyUp))
            {
                _isHotkeyDown = false;
            }
        }

        return NativeMethods.CallNextHookEx(_hookHandle, code, wParam, lParam);
    }

    private static bool IsKeyDown(int virtualKey)
    {
        return (NativeMethods.GetKeyState(virtualKey) & unchecked((short)0x8000)) != 0;
    }

    private static IntPtr GetCurrentModuleHandle()
    {
        using var currentProcess = Process.GetCurrentProcess();
        var moduleName = currentProcess.MainModule?.ModuleName;
        return string.IsNullOrWhiteSpace(moduleName)
            ? IntPtr.Zero
            : NativeMethods.GetModuleHandle(moduleName);
    }

    private static class NativeMethods
    {
        public delegate IntPtr LowLevelKeyboardProc(int code, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardHookStruct
        {
            public int VirtualKeyCode;
            public int ScanCode;
            public int Flags;
            public int Time;
            public IntPtr ExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int hookType, LowLevelKeyboardProc callback, IntPtr moduleHandle, uint threadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hookHandle);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hookHandle, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern short GetKeyState(int virtualKey);

        [DllImport("kernel32.dll", EntryPoint = "GetModuleHandleW", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr GetModuleHandle(string moduleName);
    }
}
