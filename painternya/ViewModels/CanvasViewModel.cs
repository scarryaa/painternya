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
        private Tile[,] tiles;
        private Point lastPoint;
        public int TilesX => tiles.GetLength(0);
        public int TilesY => tiles.GetLength(1);
        public int CanvasHeight { get; set; } = 1024;
        public int CanvasWidth { get; set; } = 1024;
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
            tiles = new Tile[tilesX, tilesY];

            for (int x = 0; x < tilesX; x++)
            {
                for (int y = 0; y < tilesY; y++)
                {
                    // Set every odd tile to white and every even tile to black
                    if ((x + y) % 2 == 0)
                    {
                        tiles[x, y] = new Tile(TileSize, TileSize);
                        tiles[x, y].Bitmap.Clear(Colors.White);
                    }
                    else
                    {
                        tiles[x, y] = new Tile(TileSize, TileSize);
                        tiles[x, y].Bitmap.Clear(Colors.Black);
                    }
                    
                    tiles[x, y].Dirty = true;
                    
                    // tiles[x, y] = new Tile(TileSize, TileSize);
                }
            }
        }
        
        public Tile GetTile(int x, int y)
        {
            return tiles[x, y];
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
            SetPixel((int)point.X, (int)point.Y, color);
            
            int tileX = (int)point.X / TileSize;
            int tileY = (int)point.Y / TileSize;
    
            MarkTileAsDirty(tileX, tileY);

            // If the drawing point is close to the tile edge, 
            // we'll mark neighboring tiles as dirty too.
            int pixelOffsetX = (int)point.X % TileSize;
            int pixelOffsetY = (int)point.Y % TileSize;
    
            if (pixelOffsetX < 2)
            {
                MarkTileAsDirty(tileX - 1, tileY);
            }
            if (pixelOffsetX > TileSize - 2)
            {
                MarkTileAsDirty(tileX + 1, tileY);
            }
            if (pixelOffsetY < 2)
            {
                MarkTileAsDirty(tileX, tileY - 1);
            }
            if (pixelOffsetY > TileSize - 2)
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
    
            // Check if the tile or pixel is outside the canvas
            if (tileX < 0 || tileX >= TilesX || tileY < 0 || tileY >= TilesY ||
                pixelX < 0 || pixelX >= TileSize || pixelY < 0 || pixelY >= TileSize)
            {
                return;
            }
    
            tiles[tileX, tileY].Bitmap.SetPixel(pixelX, pixelY, color);
            tiles[tileX, tileY].Dirty = true;
        }
    }
}