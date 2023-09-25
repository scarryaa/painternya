using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Media;
using painternya.Extensions;
using painternya.Interfaces;

namespace painternya.Models;

public class DrawingContext
{    
    private double _currentZoom = 1.0;
    private readonly Tile?[,]? _tiles;
    private int _totalWidth;
    private int _totalHeight;
    private readonly Subject<Unit> drawingChangedSubject;
    private readonly IOffsetObserver _observer;
    private Vector _viewport;
    
    private double OffsetX { get; set; }
    private double OffsetY { get; set; }
    private const int TileSize = 128;
    
    public double Zoom
    {
        get => _currentZoom;
        set
        {
            if (value < 0.1 || value > 10)
                return;

            _currentZoom = value;
            UpdateTileVisibilities();
        }
    }
    
    public Vector Viewport
    {
        get => _viewport;
        set
        {
            _viewport = value;
            UpdateTileVisibilities();
        }
    }
    
    public IObservable<Unit> DrawingChanged { get; private set; }

    public int TilesX => _tiles.GetLength(0);
    public int TilesY => _tiles.GetLength(1);

    public DrawingContext(IOffsetObserver offsetObserver, int totalWidth, int totalHeight)
    {
        _observer = offsetObserver;
        _observer.OffsetChanged.Subscribe(_ =>
        {
            OffsetX = _observer.OffsetX;
            OffsetY = _observer.OffsetY;
            UpdateTileVisibilities();
        });
        
        drawingChangedSubject = new Subject<Unit>();
        DrawingChanged = drawingChangedSubject.AsObservable();

        _totalWidth = totalWidth;
        _totalHeight = totalHeight;
        _viewport = new Vector(_totalWidth * .8, _totalHeight * .8);
        
        int tilesX = _totalWidth / TileSize;
        int tilesY = _totalHeight / TileSize;
            
        int remainderX = _totalWidth % TileSize;
        int remainderY = _totalHeight % TileSize;
            
        if (remainderX > 0) tilesX++;
        if (remainderY > 0) tilesY++;

        _tiles = new Tile[tilesX, tilesY];

        for (int x = 0; x < tilesX; x++)
        {
            for (int y = 0; y < tilesY; y++)
            {
                int currentTileWidth = (x == tilesX - 1 && remainderX > 0) ? remainderX : TileSize;
                int currentTileHeight = (y == tilesY - 1 && remainderY > 0) ? remainderY : TileSize;

                _tiles[x, y] = new Tile(currentTileWidth, currentTileHeight);
                    
                // Debugging code to see the tiles
                // if ((x + y) % 2 == 0)
                // {
                //     _tiles[x, y].Bitmap.Clear(Colors.White);
                // }
                // else
                // {
                //     _tiles[x, y].Bitmap.Clear(Colors.Black);
                // }

                _tiles[x, y].Dirty = true;
            }
        }
            
        UpdateTileVisibilities();
    }

    public Tile? GetTile(int x, int y)
    {
        return _tiles?[x, y];
    }
        
    public void UpdateTileVisibilities()
    {
        for (int x = 0; x < TilesX; x++)
        {
            for (int y = 0; y < TilesY; y++)
            {
                var tile = GetTile(x, y);
                if (tile != null) tile.IsVisible = IsTileVisible(x, y);
            }
        }
    }
        
    private bool IsTileVisible(int x, int y)
    {
        var tileRect = new Rect(x * TileSize, y * TileSize, TileSize, TileSize);
        var padding = 10 / _currentZoom;
        
        var viewportWidth = _viewport.X / _currentZoom;
        var viewportHeight = _viewport.Y / _currentZoom;
        
        var visibleRect = new Rect((OffsetX / _currentZoom) - padding, 
            (OffsetY / _currentZoom) - padding, 
            viewportWidth + 2 * padding, 
            viewportHeight + 2 * padding);
        
        return tileRect.Intersects(visibleRect);
    }

    public void DrawLine(Point startingPoint, Point endingPoint, Color color, int brushSize)
    {
        int x1 = (int)startingPoint.X;
        int y1 = (int)startingPoint.Y;
        int x2 = (int)endingPoint.X;
        int y2 = (int)endingPoint.Y;

        int dx = Math.Abs(x2 - x1);
        int dy = Math.Abs(y2 - y1);

        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            SetPixel(x1, y1, color, brushSize);

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

        drawingChangedSubject.OnNext(Unit.Default);
    }

    public void DrawPixel(Point point, Color color, int brushSize = 1)
    {
        if (!IsPointInsideCanvas((int)point.X, (int)point.Y))
            return;

        SetPixel((int)point.X, (int)point.Y, color, brushSize);
        MarkTileAsDirty((int)point.X / TileSize, (int)point.Y / TileSize, brushSize);

        int tileX = (int)point.X / TileSize;
        int tileY = (int)point.Y / TileSize;

        // Get the actual width and height of the current tile.
        int currentTileWidth = _tiles != null && (tileX == _tiles.GetLength(0) - 1 && _totalWidth % TileSize != 0)
            ? _totalWidth % TileSize
            : TileSize;
        int currentTileHeight = (tileY == _tiles.GetLength(1) - 1 && _totalHeight % TileSize != 0)
            ? _totalHeight % TileSize
            : TileSize;

        // Calculate pixel offset using the current tile's dimensions
        int pixelOffsetX = (int)point.X % currentTileWidth;
        int pixelOffsetY = (int)point.Y % currentTileHeight;

        // If the drawing point is close to the tile edge, 
        // we'll mark neighboring tiles as dirty too.
        if (pixelOffsetX < 2)
        {
            MarkTileAsDirty(tileX - 1, tileY);
        }

        if (pixelOffsetX > currentTileWidth - 2)
        {
            MarkTileAsDirty(tileX + 1, tileY);
        }

        if (pixelOffsetY < 2)
        {
            MarkTileAsDirty(tileX, tileY - 1);
        }

        if (pixelOffsetY > currentTileHeight - 2)
        {
            MarkTileAsDirty(tileX, tileY + 1);
        }

        drawingChangedSubject.OnNext(Unit.Default);
    }

    private void MarkTileAsDirty(int x, int y, int brushSize = 1)
    {
        int halfBrush = brushSize / 2;

        for (int offsetX = -halfBrush; offsetX <= halfBrush; offsetX++)
        {
            for (int offsetY = -halfBrush; offsetY <= halfBrush; offsetY++)
            {
                int tileX = x + offsetX;
                int tileY = y + offsetY;

                if (tileX >= 0 && tileX < TilesX && tileY >= 0 && tileY < TilesY)
                {
                    GetTile(tileX, tileY).Dirty = true;
                }
            }
        }
    }

    private void SetPixel(int x, int y, Color color, int thickness = 1)
    {
        int radius = thickness / 2;
        int squaredRadius = radius * radius;

        int maxX = _tiles?.GetLength(0) - 1 ?? -1;
        int maxY = _tiles?.GetLength(1) - 1 ?? -1;

        int modWidth = _totalWidth % TileSize;
        int modHeight = _totalHeight % TileSize;

        for (int offsetX = -radius; offsetX <= radius; offsetX++)
        {
            for (int offsetY = -radius; offsetY <= radius; offsetY++)
            {
                if (offsetX * offsetX + offsetY * offsetY > squaredRadius)
                    continue;

                int paintX = x + offsetX;
                int paintY = y + offsetY;

                if (!IsPointInsideCanvas(paintX, paintY))
                    continue;

                int tileX = paintX / TileSize;
                int tileY = paintY / TileSize;

                int pixelX = paintX % TileSize;
                int pixelY = paintY % TileSize;

                bool isPartialTileX = tileX == maxX && modWidth != 0;
                bool isPartialTileY = tileY == maxY && modHeight != 0;

                if ((isPartialTileX && pixelX >= modWidth) || 
                    (isPartialTileY && pixelY >= modHeight))
                    continue;

                _tiles[tileX, tileY].Bitmap.SetPixel(pixelX, pixelY, color);
                _tiles[tileX, tileY].Dirty = true;
            }
        }
    }

    private bool IsPointInsideCanvas(int x, int y)
    {
        return x >= 0 && x < _totalWidth && y >= 0 && y < _totalHeight;
    }
}