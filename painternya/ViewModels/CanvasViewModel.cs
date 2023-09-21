using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using painternya.Extensions;
using painternya.Models;

namespace painternya.ViewModels
{
    public class CanvasViewModel : ViewModelBase
    {
        private const int TileSize = 128;
        private Tile[,] tiles;
        public int TilesX => tiles.GetLength(0);
        public int TilesY => tiles.GetLength(1);

        public CanvasViewModel(int canvasWidth, int canvasHeight)
        {
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
        
        public void SetPixel(int x, int y, Color color)
        {
            int tileX = x / TileSize;
            int tileY = y / TileSize;
            int pixelX = x % TileSize;
            int pixelY = y % TileSize;
            
            // Check if the pixel is outside the canvas
            if (tileX < 0 || tileX >= TilesX || tileY < 0 || tileY >= TilesY)
            {
                return;
            }
            
            tiles[tileX, tileY].Bitmap.SetPixel(pixelX, pixelY, color);
            tiles[tileX, tileY].Dirty = true;
        }
    }
}