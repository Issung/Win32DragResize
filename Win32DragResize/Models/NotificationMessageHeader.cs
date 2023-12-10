using System.Runtime.InteropServices;

namespace Win32DragResize;

/// <summary>
/// AKA "NMHDR".<br/>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-nmhdr"></see>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct NotificationMessageHeader
{
    public nint hwndFrom;
    public uint idFrom;
    public uint code;
}