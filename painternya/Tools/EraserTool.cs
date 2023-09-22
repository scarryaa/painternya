using Avalonia;
using Avalonia.Media;
using painternya.Interfaces;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.Tools;

public class EraserTool : ITool
{
    public string Name { get; } = "Eraser";
    public string Icon { get; } = "M0,0H24V24H0V0ZM2,2V22H22V2H2ZM4,4H20V20H4V4Z";
    public void OnPointerPressed(DrawingContext drawingContext, Point point)
    {
        drawingContext.DrawPixel(point, Colors.Transparent);
    }

    public void OnPointerMoved(DrawingContext drawingContext, Point point)
    {
        drawingContext.DrawLine(point, point, Colors.Transparent);
    }

    public void OnPointerReleased(DrawingContext drawingContext, Point point)
    {
    }
}