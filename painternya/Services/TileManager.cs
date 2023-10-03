using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using painternya.Extensions;
using painternya.Models;

namespace painternya.Services
{
    public class TileManager
    {
        private readonly Dictionary<Point, Tile> _tiles = new();
        private readonly OrderedDictionary _loadedTiles = new();
        private readonly int _maxLoadedTiles;
        
        public static int TileSize = 1280;
        public TileManager(int totalWidth, int totalHeight, int maxLoadedTiles = 100)
        {
            _maxLoadedTiles = maxLoadedTiles;
            InitializeTiles(totalWidth, totalHeight);
        }

        public void MarkTileDirty(int x, int y)
        {
            var key = (x, y);
            
            if (_loadedTiles.Contains(key))
            {
                ((Tile)_loadedTiles[key]).Dirty = true;
            }
        }

        public void ClearAllTiles(Color color)
        {
            foreach (var tile in _tiles)
            {
                tile.Value.Bitmap.Clear(color);
            }
        }
        
        public void ClearTile(int x, int y)
        {
            var key = (x, y);
            
            if (_loadedTiles.Contains(key))
            {
                ((Tile)_loadedTiles[key]).Bitmap.Clear(Colors.Transparent);
            }
        }
        
        public Dictionary<Point,Tile> GetAllTiles()
        {
            return _tiles;
        }
        
        public void InitializeTiles(int totalWidth, int totalHeight)
        {
            int tilesInWidth = (int)Math.Ceiling((double)totalWidth / TileSize);
            int tilesInHeight = (int)Math.Ceiling((double)totalHeight / TileSize);

            for (var x = 0; x < tilesInWidth; x++)
            {
                for (var y = 0; y < tilesInHeight; y++)
                {
                    int tileWidth = (x == tilesInWidth - 1 && totalWidth % TileSize != 0) ? totalWidth % TileSize : TileSize;
                    int tileHeight = (y == tilesInHeight - 1 && totalHeight % TileSize != 0) ? totalHeight % TileSize : TileSize;

                    var tile = new Tile(tileWidth, tileHeight) { X = x, Y = y, Dirty = true };
                    _tiles[new Point(x, y)] = tile;
                }
            }
        }
        
        public RenderTargetBitmap CaptureThumbnail()
        {
            var fullSize = new RenderTargetBitmap(new PixelSize(_tiles.Count * TileSize, _tiles.Count * TileSize));

            using (var ctx = fullSize.CreateDrawingContext())
            {
                foreach (var kvp in _tiles)
                {
                    var point = kvp.Key;
                    var tile = kvp.Value;
                    ctx.DrawImage(tile.Bitmap, new Rect(point.X * TileSize, point.Y * TileSize, TileSize, TileSize));
                }
            }
    
            return fullSize;
        }
        
        public Tile GetTile(int x, int y)
        {
            var key = new Point(x, y);
            if (!_tiles.TryGetValue(key, out var tile))
            {
                tile = new Tile(TileSize, TileSize);
                _tiles[key] = tile;
            }
            return tile;
        }
        
        public void SetBitmapToTile(int tileX, int tileY, WriteableBitmap bitmap)
        {
            // Verify bitmap is of appropriate size for a tile, or resize/crop as needed.
            // ...

            // Get the tile at tileX, tileY.
            Tile tile = GetTile(tileX, tileY);

            // Now, assuming Tile has a method/property to accept a bitmap, set it.
            tile.SetBitmap(bitmap); // This method must be implemented in your Tile class.

            // If needed, mark this tile as dirty to ensure it is redrawn.
            MarkTileDirty(tileX, tileY);
        }

    }
}