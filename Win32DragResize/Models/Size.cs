using Win32DragResize.Models.Win32;

namespace Win32DragResize.Models;

public record Dimensions
{
    public int Width;
    public int Height;

    public Dimensions(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public Dimensions(Rect rect)
    { 
        Width = Util.Diff(rect.Left, rect.Right);
        Height = Util.Diff(rect.Top, rect.Bottom);
    }

    public override string? ToString()
    {
        return $"Dimensions [Width = {Width}, Height = {Height}]";
    }
}
