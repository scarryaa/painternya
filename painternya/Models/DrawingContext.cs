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
        drawingChangedSubject.OnNext(Unit.Default);
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

    public void DrawLine(Point startingPoint, Point endingPoint, Color color)
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
            SetPixel(x1, y1, color);

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

    public void DrawPixel(Point point, Color color)
    {
        if (!IsPointInsideCanvas((int)point.X, (int)point.Y))
            return;

        SetPixel((int)point.X, (int)point.Y, color);

        int tileX = (int)point.X / TileSize;
        int tileY = (int)point.Y / TileSize;

        MarkTileAsDirty(tileX, tileY);

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

    private void MarkTileAsDirty(int x, int y)
    {
        if (x >= 0 && x < TilesX && y >= 0 && y < TilesY)
        {
            GetTile(x, y).Dirty = true;
        }
    }

    private void SetPixel(int x, int y, Color color)
    {
        int tileX = x / TileSize;
        int tileY = y / TileSize;

        int pixelX = x % TileSize;
        int pixelY = y % TileSize;

        // Ensure the point is inside the current tile
        if (pixelX < 0 || pixelX >= TileSize || pixelY < 0 || pixelY >= TileSize)
            return;

        // Check if it's a partial tile by seeing if it's the last tile AND 
        // if the CanvasWidth/Height isn't an exact multiple of TileSize
        bool isPartialTileX = tileX == _tiles?.GetLength(0) - 1 && _totalWidth % TileSize != 0;
        bool isPartialTileY = tileY == _tiles?.GetLength(1) - 1 && _totalHeight % TileSize != 0;

        if ((isPartialTileX && pixelX >= _totalWidth % TileSize) ||
            (isPartialTileY && pixelY >= _totalHeight % TileSize))
            return;

        _tiles[tileX, tileY].Bitmap.SetPixel(pixelX, pixelY, color);
        _tiles[tileX, tileY].Dirty = true;
    }

    private bool IsPointInsideCanvas(int x, int y)
    {
        return x >= 0 && x < _totalWidth && y >= 0 && y < _totalHeight;
    }
}