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
    public string CurrentToolName { get; } = Name;
    public int Size { get; set; } = 4;
    
    public void OnPointerPressed(DrawingContext drawingContext, Point point, int brushSize)
    {
        drawingContext.DrawPixel(point, Colors.Transparent, brushSize);
        
        LastPoint = point;
    }

    public void OnPointerMoved(DrawingContext drawingContext, Point point, int brushSize)
    {
        drawingContext.DrawLine(LastPoint, point, Colors.Transparent, brushSize);
        
        LastPoint = point;
    }

    public void OnPointerReleased(DrawingContext drawingContext, Point point)
    {
    }
}