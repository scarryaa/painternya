using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Media;
using painternya.Actions;
using painternya.Extensions;
using painternya.Interfaces;
using painternya.Services;

namespace painternya.Models;

public class DrawingContext
{    
    private double _currentZoom = 1.0;
    private readonly LayerManager _layerManager;
    private TileManager CurrentTileManager => _layerManager.ActiveLayer.TileManager;
    private int _totalWidth;
    private int _totalHeight;
    private readonly Subject<Unit> _drawingChangedSubject;
    private readonly IOffsetObserver _observer;
    private Vector _viewport;
    
    private double OffsetX { get; set; }
    private double OffsetY { get; set; }
    
    public double Zoom
    {
        get => _currentZoom;
        set
        {
            if (value < 0.1 || value > 10)
                return;

            _currentZoom = value;

        }
    }
    
    public Vector Viewport
    {
        get => _viewport;
        set
        {
            _viewport = value;
            // UpdateTileVisibilities();
        }
    }
    
    public LayerManager LayerManager => _layerManager;
    
    public IObservable<Unit> DrawingChanged { get; private set; }

    public DrawingContext(LayerManager layerManager, IOffsetObserver offsetObserver, int totalWidth, int totalHeight)
    {
        _layerManager = layerManager;
        
        _observer = offsetObserver;
        _observer.OffsetChanged.Subscribe(_ =>
        {
            OffsetX = _observer.OffsetX;
            OffsetY = _observer.OffsetY;
            // UpdateTileVisibilities();
        });
        
        _drawingChangedSubject = new Subject<Unit>();
        DrawingChanged = _drawingChangedSubject.AsObservable();

        _totalWidth = totalWidth;
        _totalHeight = totalHeight;
        _viewport = new Vector(_totalWidth * .8, _totalHeight * .8);
    }
    
    public void DrawLine(Point startingPoint, Point endingPoint, Color color, int brushSize)
    {
        DrawSegment(startingPoint, endingPoint, color, brushSize);
        _drawingChangedSubject.OnNext(Unit.Default);
    }

    public void DrawLine(Point startingPoint, List<Point> points, Color color, int brushSize)
    {
        if (points.Count == 0) return;

        Point currentPoint = startingPoint;
        foreach (Point endingPoint in points)
        {
            DrawSegment(currentPoint, endingPoint, color, brushSize);
            currentPoint = endingPoint;
        }
        _drawingChangedSubject.OnNext(Unit.Default);
    }
    
    private bool IsPointInsideCanvas(double x, double y)
    {
        return x >= 0 && x < _totalWidth && y >= 0 && y < _totalHeight;
    }

    private void DrawSegment(Point startPoint, Point endPoint, Color color, int brushSize)
    {
        int x1 = (int)startPoint.X;
        int y1 = (int)startPoint.Y;
        int x2 = (int)endPoint.X;
        int y2 = (int)endPoint.Y;

        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);

        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            SetPixel(new Point(x1, y1), color, brushSize);

            if (x1 == x2 && y1 == y2) break;

            int e2 = 2 * err;

            if (e2 > -dy)
            {
                err -= dy;
                x1 += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y1 += sy;
            }
        }
    }

    public void SetPixel(Point point, Color color, int thickness = 1)
    {
        int radius = thickness / 2;
        int squaredRadius = radius * radius;

        int x = (int)point.X;
        int y = (int)point.Y;
    
        for (int offsetX = -radius; offsetX <= radius; offsetX++)
        {
            for (int offsetY = -radius; offsetY <= radius; offsetY++)
            {
                if (offsetX * offsetX + offsetY * offsetY <= squaredRadius) 
                {
                    int absoluteX = x + offsetX;
                    int absoluteY = y + offsetY;
                    
                    if (!IsPointInsideCanvas(absoluteX, absoluteY))
                    {
                        continue;
                    }

                    int tileX = absoluteX / TileManager.TileSize;
                    int tileY = absoluteY / TileManager.TileSize;
                
                    var tile = CurrentTileManager.GetTile(tileX, tileY);
                
                    int pixelX = absoluteX % TileManager.TileSize;
                    int pixelY = absoluteY % TileManager.TileSize;

                    tile.Bitmap.SetPixel(pixelX, pixelY, color);
                    tile.Dirty = true;
                }
            }
        }
    }

    public void Execute(DrawLineAction lineAction)
    {
        lineAction.Execute();
    }
}