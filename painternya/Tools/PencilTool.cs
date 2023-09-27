using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using painternya.Interfaces;
using DrawingContext = painternya.Models.DrawingContext;
using System.Collections.Generic;
using painternya.Actions;

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

        public PencilTool(int size)
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
            
            overlayContext.DrawLine(LastPoint, point, Colors.Black, Size);
            
            LastPoint = point;
        }

        public void OnPointerReleased(DrawingContext overlayContext, DrawingContext drawingContext, Point point)
        {
            if (AccumulatedPoints.Count > 1)
            {
                drawingContext.DrawLine(StartPoint, AccumulatedPoints, Colors.Black, Size);

                AccumulatedPoints.Clear();
            }
        }
    }
}
