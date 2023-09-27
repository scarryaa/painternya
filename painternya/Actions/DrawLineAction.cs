using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.Actions;

public class DrawLineAction
{
    private readonly DrawingContext _drawingContext;
    private readonly Point _startPoint;
    private readonly List<Point> _accumulatedPoints;
    private readonly Color _black;
    public int Size { get; set; }
    
    public DrawLineAction(DrawingContext drawingContext, Point startPoint, List<Point> accumulatedPoints, Color black, int size)
    {
        _drawingContext = drawingContext;
        _startPoint = startPoint;
        _accumulatedPoints = accumulatedPoints;
        _black = black;
        Size = size;
    }

    public void Execute()
    {
        _drawingContext.DrawLine(_startPoint, _accumulatedPoints, _black, Size);
    }
}