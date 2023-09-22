using Avalonia;
using Avalonia.Media;
using painternya.Interfaces;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.Tools;

public class EraserTool : ITool
{
    public Point LastPoint { get; set; }
    public static string Name { get; } = "Eraser";
    public static string Icon { get; } = "fa-solid fa-eraser";
    public void OnPointerPressed(DrawingContext drawingContext, Point point)
    {
        drawingContext.DrawPixel(point, Colors.Transparent);
        
        LastPoint = point;
    }

    public void OnPointerMoved(DrawingContext drawingContext, Point point)
    {
        drawingContext.DrawLine(LastPoint, point, Colors.Transparent);
        
        LastPoint = point;
    }

    public void OnPointerReleased(DrawingContext drawingContext, Point point)
    {
    }
}