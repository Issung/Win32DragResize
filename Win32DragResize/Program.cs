using Gma.System.MouseKeyHook;
using System.Diagnostics;
using Win32DragResize.Models;
using Win32DragResize.Models.Win32;

namespace Win32DragResize;

/// <summary>
/// Resources:
/// <list type="bullet">
/// <item>Get/Set WindowPlacement: <see href="https://stackoverflow.com/a/4894799/8306962"/></item>
/// </list>
/// </summary>
partial class Program
{
    // https://learn.microsoft.com/en-us/windows/win32/winauto/event-constants
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
    public const uint SETWINDOWRECT_FLAGS = SWP_DRAWFRAME;

    /// <summary>
    /// Keep the delegate instance alive to prevent it from being garbage collected
    /// </summary>
    private static readonly Win32.WinEventProc winEventDelegate = WindowMoveSizeCallback;
    private static readonly IKeyboardMouseEvents keyboardAndMouseHooks = Hook.GlobalEvents();

    static nint? draggingHwnd;
    static Rect draggingRect;
    static Dimensions size;

    static void Main()
    {
        // Install the hook for location change events
        var hook = Win32.SetWinEventHook(EVENT_SYSTEM_MOVESIZESTART, EVENT_SYSTEM_MOVESIZEEND, nint.Zero, winEventDelegate, 0, 0, 0);
        keyboardAndMouseHooks.MouseWheel += GlobalHook_MouseWheel;
        keyboardAndMouseHooks.KeyDown += KeyboardAndMouseHooks_KeyDown;

        Application.Run();

        // Unhook when done
        Win32.UnhookWinEvent(hook);
    }

    // TODO: If scrolling while holding SHIFT, only resize horizontally.
    private static void KeyboardAndMouseHooks_KeyDown(object? sender, KeyEventArgs e)
    {
        Debug.WriteLine("Key down");
        if (e.Control)
        {
            Debug.WriteLine("Control down");
            GlobalHook_MouseWheel(null, null!);
        }
    }

    /// <summary>
    /// Called once when drag starts, and once when finished. Discerned by <paramref name="eventType"/>.
    /// </summary>
    private static void WindowMoveSizeCallback(nint hWinEventHook, uint eventType, nint hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        //Debug.WriteLine($"hWinEventHook: {hWinEventHook}, eventType: {eventType}, hwnd: {hwnd}.");
        if (eventType == EVENT_SYSTEM_MOVESIZESTART)
        {
            // Log information about the window's position change
            //Debug.WriteLine($"Window location changed. Left: {rect.left}, Top: {rect.top}");
            Win32.GetWindowRect(hwnd, out draggingRect);
            Debug.WriteLine($"Saving hwnd {hwnd}. Rect Left: {draggingRect.Left}, Top: {draggingRect.Top}, Right: {draggingRect.Right}, Bottom: {draggingRect.Bottom}");
            draggingHwnd = hwnd;
            size = new(draggingRect);
            Debug.WriteLine(size.ToString());
        }
        else if (eventType == EVENT_SYSTEM_MOVESIZEEND)
        {
            Win32.GetWindowRect(draggingHwnd.Value, out var currentRect);
            var (middleX, _) = Util.GetMiddle(currentRect);
            var left = middleX - (size.Width / 2);
            Win32.SetWindowPos(
                draggingHwnd.Value,
                nint.Zero,
                left: left,
                top: currentRect.Top,
                width: size.Width,
                height: size.Height,
                uFlags: SETWINDOWRECT_FLAGS
            );

            draggingHwnd = null;
        }
    }

    private static void GlobalHook_MouseWheel(object? sender, MouseEventArgs e)
    {
        Debug.WriteLine("GlobalHook_MouseWheel");

        if (draggingHwnd != null)
        {
            const int amount = 25;
            var adjustment = e?.Delta is null or < 0 ? -amount : amount;

            Win32.GetWindowRect(draggingHwnd.Value, out var currentRect);
            //Debug.WriteLine($"currentRect Left: {currentRect.Left}, Top: {currentRect.Top}, Right: {currentRect.Right}, Bottom: {currentRect.Bottom}.");

            var (middleX, _) = Util.GetMiddle(currentRect);
            //Debug.WriteLine($"middleX: {middleX}, middleY: {middleY}.");

            size.Width += adjustment;// draggingRect.Right - draggingRect.Left;
            size.Height += adjustment; // draggingRect.Bottom - draggingRect.Top;
            //Debug.WriteLine(size.ToString());

            var left = middleX - (size.Width / 2);
            //Debug.WriteLine($"Desires: Left {left}, Right: {left + size.Width}, Top: {top}, Bottom: {top + size.Height}.");

            Win32.SetWindowPos(
                draggingHwnd.Value,
                nint.Zero,
                left: left,
                top: currentRect.Top,
                width: size.Width,
                height: size.Height,
                uFlags: SETWINDOWRECT_FLAGS
            );
        }
        else
        {
            //Debug.WriteLine($"MouseWheel! Delta: {e.Delta}.");
        }
    }
}