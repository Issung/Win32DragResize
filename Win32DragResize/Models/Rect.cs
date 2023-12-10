using System.Runtime.InteropServices;

namespace Win32DragResize;

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-rect"></see>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
    public int left;
    public int top;
    public int right;
    public int bottom;
}
