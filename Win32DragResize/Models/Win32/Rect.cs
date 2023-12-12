using System.Runtime.InteropServices;

namespace Win32DragResize.Models.Win32;

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-rect"></see>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Rect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}
