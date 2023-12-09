using Gma.System.MouseKeyHook;
using System.Diagnostics;
using System.Runtime.InteropServices;

/// <summary>
/// Resources:
/// <list type="bullet">
/// <item>Get/Set WindowPlacement: <see href="https://stackoverflow.com/a/4894799/8306962"/></item>
/// </list>
/// </summary>
class Program
{
    // https://learn.microsoft.com/en-us/windows/win32/winauto/event-constants
    private const uint EVENT_OBJECT_LOCATIONCHANGE = 0x800B;
    private const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;
    private const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

    // Consts for use with SetWindowPos https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos?redirectedfrom=MSDN
    public const uint SWP_ASYNCWINDOWPOS = 0x4000;
    public const uint SWP_DEFERERASE = 0x2000;
    public const uint SWP_DRAWFRAME = 0x0020;
    public const uint SWP_FRAMECHANGED = 0x0020;
    public const uint SWP_HIDEWINDOW = 0x0080;
    public const uint SWP_NOACTIVATE = 0x0010;
    public const uint SWP_NOCOPYBITS = 0x0100;
    public const uint SWP_NOMOVE = 0x0002;
    public const uint SWP_NOOWNERZORDER = 0x0200;
    public const uint SWP_NOREDRAW = 0x0008;
    public const uint SWP_NOREPOSITION = 0x0200;
    public const uint SWP_NOSENDCHANGING = 0x0400;
    public const uint SWP_NOSIZE = 0x0001;
    public const uint SWP_NOZORDER = 0x0004;
    public const uint SWP_SHOWWINDOW = 0x0040;

    // ???
    public const int WM_USER = 0x0400;
    public const int WM_RESIZE_FINISHED = WM_USER + 1;
    public const int WM_WINDOWPOSCHANGING = 0x0046;
    public const int WM_WINDOWPOSCHANGED = 0x0047;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);


    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    // Define the callback delegate type for the WinEventDelegate
    private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

    private delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// Keep the delegate instance alive to prevent it from being garbage collected
    /// </summary>
    private static readonly WinEventDelegate winEventDelegate = WinEventCallback;

    private static IKeyboardMouseEvents keyboardAndMouseHooks = Hook.GlobalEvents();

    static IntPtr? hwnd;
    static RECT rect;

    static void Main()
    {
        // Install the hook for location change events
        IntPtr hook = SetWinEventHook(EVENT_SYSTEM_MOVESIZESTART, EVENT_SYSTEM_MOVESIZEEND, IntPtr.Zero, winEventDelegate, 0, 0, 0);
        keyboardAndMouseHooks.MouseWheel += GlobalHook_MouseWheel;
        keyboardAndMouseHooks.KeyDown += KeyboardAndMouseHooks_KeyDown;

        Application.Run();

        // Unhook when done
        UnhookWinEvent(hook);
    }

    private static void KeyboardAndMouseHooks_KeyDown(object? sender, KeyEventArgs e)
    {
        Debug.WriteLine("Key down");
        if (e.Control)
        {
            Debug.WriteLine("Control down");
            GlobalHook_MouseWheel(null, null!);
        }
    }

    private static void WinEventCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        //Debug.WriteLine($"hWinEventHook: {hWinEventHook}, eventType: {eventType}, hwnd: {hwnd}.");
        if (eventType == EVENT_SYSTEM_MOVESIZESTART)
        {
            // Log information about the window's position change
            //Debug.WriteLine($"Window location changed. Left: {rect.left}, Top: {rect.top}");
            GetWindowRect(hwnd, out rect);
            Debug.WriteLine($"Saving hwnd {hwnd}");
            Program.hwnd = hwnd;
        }
        else if (eventType == EVENT_SYSTEM_MOVESIZEEND)
        {
            //Program.hwnd = null;
        }
    }

    private static void GlobalHook_MouseWheel(object? sender, MouseEventArgs e)
    {
        Debug.WriteLine("GlobalHook_MouseWheel");
        if (hwnd != null)
        {
            //var flags = SWP_ASYNCWINDOWPOS | SWP_DRAWFRAME | SWP_SHOWWINDOW | SWP_NOCOPYBITS | SWP_NOREPOSITION/*| SWP_NOSENDCHANGING*/;
            //var flags = SWP_NOZORDER | SWP_NOREPOSITION;
            uint flags = SWP_DRAWFRAME;

            Debug.WriteLine($"MouseWheel while dragging.");
            const int amount = 25;
            var adjustment = e?.Delta is null or < 0 ? -amount : amount;

            //SendMessage(hwnd.Value, WM_WINDOWPOSCHANGING, IntPtr.Zero, IntPtr.Zero);

            GetWindowRect(hwnd.Value, out var currentRect);
            var width = currentRect.right - currentRect.left;
            var height = currentRect.bottom - currentRect.top;

            SetWindowPos(hwnd.Value, IntPtr.Zero,
                x: currentRect.left - adjustment / 2,
                y: currentRect.top,
                cx: width + adjustment,
                cy: height + adjustment,
                uFlags: flags);

            //SendMessage(hwnd.Value, WM_WINDOWPOSCHANGED, IntPtr.Zero, IntPtr.Zero);
            //MoveWindow(hwnd.Value, 50, 50, 500, 500, true);
        }
        else
        { 
            //Debug.WriteLine($"MouseWheel! Delta: {e.Delta}.");
            Debug.WriteLine($"MouseWheel.");
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int X;
        public int Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    // Import necessary method to get window rectangle
    [DllImport("user32.dll")]
    private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool AdjustWindowRect(ref RECT lpRect, uint dwStyle, [MarshalAs(UnmanagedType.Bool)] bool bMenu);

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos?redirectedfrom=MSDN"/>
    /// </summary>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    /// <summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-movewindow"/>
    /// </summary>
    /// <param name="bRepaint"></param>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

    [StructLayout(LayoutKind.Sequential)]
    public struct NMHDR
    {
        public IntPtr hwndFrom;
        public uint idFrom;
        public uint code;
    }
}
