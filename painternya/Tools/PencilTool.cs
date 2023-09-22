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