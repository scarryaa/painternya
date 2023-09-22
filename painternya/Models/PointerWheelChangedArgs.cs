using Avalonia;

namespace painternya.Models;

public class PointerWheelChangedArgs
{
    private readonly double _delta;
    private readonly Point _position;
    private readonly bool _isControlDown;
    private readonly bool _isShiftDown;
    
    public double Delta => _delta;
    public Point Position => _position;
    public bool IsControlDown => _isControlDown;
    public bool IsShiftDown => _isShiftDown;

    public PointerWheelChangedArgs(double delta, Point position, bool isControlDown, bool isShiftDown)
    {
        _delta = delta;
        _position = position;
        _isControlDown = isControlDown;
        _isShiftDown = isShiftDown;
    }
}