using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using painternya.Actions;
using painternya.Interfaces;
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
    
    public void OnPointerPressed(DrawingContext drawingContext, Point point, int brushSize)
    {
        StartPoint = point;
        AccumulatedPoints.Clear();
        AccumulatedPoints.Add(point);
            
        LastPoint = point;
    }

    public void OnPointerMoved(DrawingContext overlayContext, DrawingContext drawingContext, Point point, int brushSize)
    {
        
        AccumulatedPoints.Add(point);
            
        overlayContext.DrawLine(LastPoint, point, Colors.Transparent, Size);
            
        LastPoint = point;
    }

    public void OnPointerReleased(DrawingContext overlayContext, DrawingContext drawingContext, Point point)
    {
        if (AccumulatedPoints.Count > 1)
        {
            var lineAction = new DrawLineAction(drawingContext, StartPoint, AccumulatedPoints, Colors.Transparent, Size);
            drawingContext.Execute(lineAction);
                
            AccumulatedPoints.Clear();
        }
    }
}