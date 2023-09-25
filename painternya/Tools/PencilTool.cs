using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using painternya.Interfaces;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.Tools;

public class PencilTool : ITool
{
    public Point LastPoint { get; set; }
    public static string Name { get; } = "Pencil";
    public static string Icon { get; } = "fa-solid fa-pencil";
    public string CurrentToolName { get; } = Name;
    public int Size { get; set; }

    public PencilTool(int size)
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