using Win32DragResize.Models.Win32;

namespace Win32DragResize;

public static class Util
{
    public static int Diff(int a, int b)
    {
        return Math.Abs(a - b);
    }

    public static (int middleX, int middleY) GetMiddle(Rect rect)
    {
        return (GetMiddle(rect.Left, rect.Right), GetMiddle(rect.Top, rect.Bottom));
    }

    public static int GetMiddle(int a, int b)
    {
        return Math.Min(a, b) + (Diff(a, b) / 2);
    }
}