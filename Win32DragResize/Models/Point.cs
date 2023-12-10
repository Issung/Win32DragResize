using System.Runtime.InteropServices;

namespace Win32DragResize;

/// <summary>
/// <see href="https://learn.microsoft.com/en-us/windows/win32/api/windef/ns-windef-point"></see>
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Point
{
    public long X;
    public long Y;
}
