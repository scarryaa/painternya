using Avalonia;
using Avalonia.Media.Imaging;
using painternya.Models;

namespace painternya.Interfaces;

public interface ITool
{
    public string Name { get; }
    public string Icon { get; }
    public void OnPointerPressed(DrawingContext drawingContext, Point point);
    public void OnPointerMoved(DrawingContext drawingContext, Point point);
    public void OnPointerReleased(DrawingContext drawingContext, Point point);
}