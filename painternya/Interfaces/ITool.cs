using Avalonia;
using Avalonia.Media.Imaging;
using painternya.Models;

namespace painternya.Interfaces;

public interface ITool
{
    public Point LastPoint { get; set; }
    public static string Name { get; }
    public static string Icon { get; }
    public string CurrentToolName { get; }
    public int Size { get; set; }
    public void OnPointerPressed(DrawingContext drawingContext, Point point, int brushSize);
    public void OnPointerMoved(DrawingContext drawingContext, Point point, int brushSize);
    public void OnPointerReleased(DrawingContext drawingContext, Point point);
}