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
    public string CurrentToolName { get; } = Name;
    public int Size { get; set; } = 4;

    public BrushTool(int size)
    {
        Size = size;
    }

    public void OnPointerPressed(DrawingContext drawingContext, Point point, int brushSize)
    {
        drawingContext.DrawPixel(point, Colors.Black, brushSize);
        
        LastPoint = point;
    }

    public void OnPointerMoved(DrawingContext drawingContext, Point point, int brushSize)
    {
        drawingContext.DrawLine(LastPoint, point, Colors.Black, brushSize);
        
        LastPoint = point;
    }

    public void OnPointerReleased(DrawingContext drawingContext, Point point)
    {
    }
}