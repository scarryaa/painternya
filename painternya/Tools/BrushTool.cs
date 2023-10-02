using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Media;
using painternya.Interfaces;
using painternya.Models;
using painternya.Services;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.Tools;

public class BrushTool : ITool
{
    public Point LastPoint { get; set; }
    public Point StartPoint { get; set; }
    private List<Point> AccumulatedPoints { get; set; } = new();
    public static string Name { get; } = "Brush";
    public static string Icon { get; } = "fa-solid fa-paintbrush";
    public string CurrentToolName { get; } = Name;
    public int Size { get; set; } = 4;
    public Layer ActiveLayer { get; set; }

    public BrushTool(int size)
    {
        Size = size;
    }

    public void OnPointerPressed(LayerManager layerManager, DrawingContext drawingContext, Point point, int brushSize)
    {
        ActiveLayer = layerManager.ActiveLayer;
        if (ActiveLayer == null)
            return;
        
        layerManager.SetActiveLayer(layerManager.PreviewLayer);
        
        StartPoint = point;
        AccumulatedPoints.Clear();
        AccumulatedPoints.Add(point);
        LastPoint = point;
    }

    public void OnPointerMoved(DrawingContext drawingContext, Point point, int brushSize)
    {
        AccumulatedPoints.Add(point);
            
        drawingContext.DrawLine(LastPoint, point, drawingContext.CurrentColor, Size);
            
        LastPoint = point;
    }

    public void OnPointerReleased(LayerManager layerManager, DrawingContext drawingContext, Point point)
    {
        layerManager.SetActiveLayer(ActiveLayer);
        
        if (AccumulatedPoints.Count > 1)
        {
            LastPoint = AccumulatedPoints[0];
            drawingContext.DrawLine(LastPoint, AccumulatedPoints, drawingContext.CurrentColor, Size);

            AccumulatedPoints.Clear();
            layerManager.ClearPreviewLayer();
        }
    }
}