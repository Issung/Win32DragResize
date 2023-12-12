using System.Runtime.InteropServices;

namespace Win32DragResize.Models.Win32;

/// <summary>
/// AKA "NMHDR".<br/>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-nmhdr"></see>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct NotificationMessageHeader
{
    public nint HwndFrom;
    public uint IdFrom;
    public uint Code;
}