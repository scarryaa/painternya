using Avalonia;

namespace painternya.Models;

public class PointerWheelChangedArgs
{
    public double Delta { get; }
    public Point Position { get; }
    public bool IsControlDown { get; }
    public bool IsShiftDown { get; }

    public PointerWheelChangedArgs(double delta, Point position, bool isControlDown, bool isShiftDown)
    {
        Delta = delta;
        Position = position;
        IsControlDown = isControlDown;
        IsShiftDown = isShiftDown;
    }
}