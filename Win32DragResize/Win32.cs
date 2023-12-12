using System.Runtime.InteropServices;
using Win32DragResize.Models.Win32;

namespace Win32DragResize;

public static class Win32
{
    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa"></see>
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwineventhook"></see>
    /// </summary>
    [DllImport("user32.dll")]
    public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    public static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    [DllImport("kernel32.dll")]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool AdjustWindowRect(ref Rect lpRect, uint dwStyle, [MarshalAs(UnmanagedType.Bool)] bool bMenu);

    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(nint hwnd, out Rect lpRect);

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos?redirectedfrom=MSDN"/>
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool SetWindowPos(nint hWnd, nint hWndInsertAfter, int left, int top, int width, int height, uint uFlags);

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-movewindow"/>
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool MoveWindow(nint hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern nint SendMessage(nint hWnd, uint msg, nint wParam, nint lParam);

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-wineventproc"/>
    /// </summary>
    public delegate void WinEventProc(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint idEventThread, uint dwmsEventTime);

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nc-winuser-hookproc"></see>
    /// </summary>
    public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);
}
