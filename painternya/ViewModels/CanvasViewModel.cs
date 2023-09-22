using System;
using System.Reactive;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using painternya.Extensions;
using painternya.Models;
using ReactiveUI;

namespace painternya.ViewModels
{
    public class CanvasViewModel : ViewModelBase
    {
        private const int TileSize = 128;
        private Tile[,] _tiles;
        private Point lastPoint;
        private int _canvasHeight;
        private int _canvasWidth;
        public int TilesX => _tiles.GetLength(0);
        public int TilesY => _tiles.GetLength(1);
        public int CanvasWidth
        {
            get => _canvasWidth;
            set
            {
                _canvasWidth = value;
                this.RaisePropertyChanged();
            }
        }
        
        public int CanvasHeight
        {
            get => _canvasHeight;
            set
            {
                _canvasHeight = value;
                this.RaisePropertyChanged();
            }
        }
        public double HorizontalOffset { get; set; }
        public double VerticalOffset { get; set; }
        public ICommand PointerMovedCommand { get; set; }
        public ICommand PointerPressedCommand { get; set; }
        public event Action InvalidateRequested;
        
        public CanvasViewModel() {}
        
        public CanvasViewModel(int canvasWidth, int canvasHeight)
        {
            PointerMovedCommand = ReactiveCommand.Create<Point>(HandlePointerMoved);
            PointerPressedCommand = ReactiveCommand.Create<Point>(HandlePointerPressed);
            
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;
            
            int tilesX = canvasWidth / TileSize;
            int tilesY = canvasHeight / TileSize;
            
            int remainderX = canvasWidth % TileSize;
            int remainderY = canvasHeight % TileSize;
            
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
        
        public Tile GetTile(int x, int y)
        {
            return _tiles[x, y];
        }
        
        public void UpdateTileVisibilities()
        {
            for (int x = 0; x < TilesX; x++)
            {
                for (int y = 0; y < TilesY; y++)
                {
                    var tile = GetTile(x, y);
                    tile.IsVisible = IsTileVisible(x, y);
                }
            }
            InvalidateRequested?.Invoke();
        }
        
        private bool IsTileVisible(int x, int y)
        {
            var tileRect = new Rect(x * TileSize, y * TileSize, TileSize, TileSize);
            var visibleRect = new Rect(HorizontalOffset, VerticalOffset, CanvasWidth, CanvasHeight);

            return tileRect.Intersects(visibleRect);
        }


        private void HandlePointerPressed(Point point)
        {
            DrawPixel(point, Colors.Red);
            lastPoint = point;
        }
        
        private void HandlePointerMoved(Point point)
        {
            DrawLine(lastPoint, point, Colors.Red);
            lastPoint = point;
        }

        private void DrawLine(Point startingPoint, Point endingPoint, Color color)
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
            
            InvalidateRequested?.Invoke();
        }


        private void DrawPixel(Point point, Color color)
        {
            if (!IsPointInsideCanvas((int)point.X, (int)point.Y))
                return;
    
            SetPixel((int)point.X, (int)point.Y, color);

            int tileX = (int)point.X / TileSize;
            int tileY = (int)point.Y / TileSize;

            MarkTileAsDirty(tileX, tileY);

            // Get the actual width and height of the current tile.
            int currentTileWidth = (tileX == _tiles.GetLength(0) - 1 && CanvasWidth % TileSize != 0) ? CanvasWidth % TileSize : TileSize;
            int currentTileHeight = (tileY == _tiles.GetLength(1) - 1 && CanvasHeight % TileSize != 0) ? CanvasHeight % TileSize : TileSize;

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

            InvalidateRequested?.Invoke();
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
            bool isPartialTileX = tileX == _tiles.GetLength(0) - 1 && CanvasWidth % TileSize != 0;
            bool isPartialTileY = tileY == _tiles.GetLength(1) - 1 && CanvasHeight % TileSize != 0;

            if ((isPartialTileX && pixelX >= CanvasWidth % TileSize) || 
                (isPartialTileY && pixelY >= CanvasHeight % TileSize))
                return;

            _tiles[tileX, tileY].Bitmap.SetPixel(pixelX, pixelY, color);
            _tiles[tileX, tileY].Dirty = true;
        }

        
        private bool IsPointInsideCanvas(int x, int y)
        {
            return x >= 0 && x < CanvasWidth && y >= 0 && y < CanvasHeight;
        }
    }
}