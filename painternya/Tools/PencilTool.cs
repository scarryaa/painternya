using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using painternya.Interfaces;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.Tools;

public class PencilTool : ITool
{
    public string Name { get; }
    public string Icon { get; }
    
    public PencilTool(WriteableBitmap[] tiles)
    {
        Name = "Pencil";
        Icon = "M0,0H24V24H0V0ZM2,2V22H22V2H2ZM4,4H20V20H4V4Z";
    }
    
    public void OnPointerPressed(DrawingContext drawingContext, Point point)
    {
        drawingContext.DrawPixel(point, Colors.Black);
    }

    public void OnPointerMoved(DrawingContext drawingContext, Point point)
    {
        throw new System.NotImplementedException();
    }

    public void OnPointerReleased(DrawingContext drawingContext, Point point)
    {
        throw new System.NotImplementedException();
    }
}