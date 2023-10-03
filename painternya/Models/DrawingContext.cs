using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using painternya.Actions;
using painternya.Extensions;
using painternya.Interfaces;
using painternya.Services;

namespace painternya.Models;

public class DrawingContext
{
    public RenderTargetBitmap OffscreenBitmap { get; private set; }
    private List<(Point point, Color color, int thickness)> _drawBuffer = new();
    private Color _currentColor = Colors.Black;
    private readonly ThumbnailCapturer _thumbnailCapturer;
    private double _currentZoom = 1.0;
    private readonly LayerManager _layerManager;
    private TileManager? CurrentTileManager => _layerManager.ActiveLayer?.TileManager ?? null;
    private int _totalWidth;
    private int _totalHeight;
    private readonly Subject<Unit> _drawingChangedSubject;
    private readonly IOffsetObserver _observer;
    private RenderTargetBitmap renderTargetBitmap;
    private Vector _viewport;
    
    private double OffsetX { get; set; }
    private double OffsetY { get; set; }
    
    public Color CurrentColor
    {
        get => _currentColor;
        set => _currentColor = value;
    }
    
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
        renderTargetBitmap = new RenderTargetBitmap(new PixelSize(totalWidth, totalHeight));
        _layerManager = layerManager;
        _thumbnailCapturer = new ThumbnailCapturer(CurrentTileManager, totalWidth, totalHeight);
        
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
    
    public RenderTargetBitmap CaptureThumbnail()
    {
        return _thumbnailCapturer.CaptureThumbnail();
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
            _drawBuffer.Add((new Point(x1, y1), color, brushSize));

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
        
        Task.Run(BatchDraw);
    }
    
    public void BatchDraw()
    {
        if (_drawBuffer.Count == 0) return;

        using (var context = renderTargetBitmap.CreateDrawingContext())
        {
            foreach (var (point, color, thickness) in _drawBuffer)
            {
                RenderPoint(context, point, color, thickness);
            }
        }

        _drawBuffer.Clear();
        OffscreenBitmap = renderTargetBitmap;
        
        _drawingChangedSubject.OnNext(Unit.Default);
    }

    private void RenderPoint(Avalonia.Media.DrawingContext context, Point point, Color color, int thickness)
    {
        var pixelsToChange = new Dictionary<Point, Color>();
        int radius = thickness / 2;
        int squaredRadius = radius * radius;

        int x = (int)point.X;
        int y = (int)point.Y;

        if (!IsPointInsideCanvas(x, y))
        {
            return;
        }

        int minOffsetX = Math.Max(-radius, -x);
        int maxOffsetX = Math.Min(radius, _totalWidth - 1 - x);
        int minOffsetY = Math.Max(-radius, -y);
        int maxOffsetY = Math.Min(radius, _totalHeight - 1 - y);

        for (int offsetX = minOffsetX; offsetX <= maxOffsetX; offsetX++)
        {
            for (int offsetY = minOffsetY; offsetY <= maxOffsetY; offsetY++)
            {
                if (offsetX * offsetX + offsetY * offsetY <= squaredRadius)
                {
                    int absoluteX = x + offsetX;
                    int absoluteY = y + offsetY;

                    int tileX = absoluteX / TileManager.TileSize;
                    int tileY = absoluteY / TileManager.TileSize;

                    if (LayerManager.ActiveLayer == null)
                        return;

                    var tile = CurrentTileManager.GetTile(tileX, tileY);

                    int pixelX = absoluteX % TileManager.TileSize;
                    int pixelY = absoluteY % TileManager.TileSize;

                    pixelsToChange[new Point(pixelX, pixelY)] = color;
                    tile.Dirty = true;
                }
            }
        }
        
        var affectedTile = CurrentTileManager.GetTile(x / TileManager.TileSize, y / TileManager.TileSize);
        affectedTile.Bitmap.SetPixels(pixelsToChange);
    }


    public void Execute(DrawLineAction lineAction)
    {
        lineAction.Execute();
    }
}