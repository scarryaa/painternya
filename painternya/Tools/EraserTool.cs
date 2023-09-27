using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using painternya.Actions;
using painternya.Interfaces;
using painternya.Services;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.Tools;

public class EraserTool : ITool
{
    public Point LastPoint { get; set; }
    public Point StartPoint { get; set; }
    public List<Point> AccumulatedPoints { get; set; } = new List<Point>();
    public static string Name { get; } = "Eraser";
    public static string Icon { get; } = "fa-solid fa-eraser";
    public string CurrentToolName { get; } = Name;
    public int Size { get; set; } = 4;

    public EraserTool(int size)
    {
        Size = size;
    }
    
    public void OnPointerPressed(LayerManager layerManager, DrawingContext drawingContext, Point point, int brushSize)
    {
        StartPoint = point;
        AccumulatedPoints.Clear();
        AccumulatedPoints.Add(point);
            
        LastPoint = point;
    }

    public void OnPointerMoved(DrawingContext drawingContext, Point point, int brushSize)
    {
        AccumulatedPoints.Add(point);
            
        drawingContext.DrawLine(LastPoint, point, Colors.Transparent, Size);
            
        LastPoint = point;
    }

    public void OnPointerReleased(LayerManager layerManager, DrawingContext drawingContext, Point point)
    {
        {
            if (AccumulatedPoints.Count > 1)
            {
                LastPoint = AccumulatedPoints[0];
                drawingContext.DrawLine(LastPoint, AccumulatedPoints, Colors.Transparent, Size);

                AccumulatedPoints.Clear();
                layerManager.ClearPreviewLayer();
            }
        }
    }
}