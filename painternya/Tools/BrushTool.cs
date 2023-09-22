using Avalonia;
using Avalonia.Media;
using painternya.Interfaces;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.Tools;

public class BrushTool : ITool
{
    public Point LastPoint { get; set; }
    public static string Name { get; } = "Brush";
    public static string Icon { get; } = "fa-solid fa-paintbrush";
    public void OnPointerPressed(DrawingContext drawingContext, Point point)
    {
        drawingContext.DrawPixel(point, Colors.Black);
        
        LastPoint = point;
    }

    public void OnPointerMoved(DrawingContext drawingContext, Point point)
    {
        drawingContext.DrawLine(LastPoint, point, Colors.Black);
        
        LastPoint = point;
    }

    public void OnPointerReleased(DrawingContext drawingContext, Point point)
    {
    }
}