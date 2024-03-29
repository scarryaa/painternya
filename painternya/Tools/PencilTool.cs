using Avalonia;
using painternya.Interfaces;
using DrawingContext = painternya.Models.DrawingContext;
using System.Collections.Generic;
using painternya.Models;
using painternya.Services;

namespace painternya.Tools
{
    public class PencilTool : ITool
    {
        public Point LastPoint { get; set; }
        public Point StartPoint { get; set; }
        public List<Point> AccumulatedPoints { get; set; } = new List<Point>();
        public static string Name { get; } = "Pencil";
        public static string Icon { get; } = "fa-solid fa-pencil";
        public string CurrentToolName { get; } = Name;
        public int Size { get; set; }
        public Layer ActiveLayer { get; set; }

        public PencilTool(int size)
        {
            Size = 1;
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
            
            drawingContext.DrawLine(LastPoint, point, drawingContext.CurrentColor, 1);
            
            LastPoint = point;
        }

        public void OnPointerReleased(LayerManager layerManager, DrawingContext drawingContext, Point point)
        {
            layerManager.SetActiveLayer(ActiveLayer);
            if (AccumulatedPoints.Count > 1)
            {
                LastPoint = AccumulatedPoints[0];
                drawingContext.DrawLine(LastPoint, AccumulatedPoints, drawingContext.CurrentColor, 1);

                AccumulatedPoints.Clear();
                layerManager.ClearPreviewLayer();
            }
        }
    }
}
