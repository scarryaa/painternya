using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Media;
using painternya.Extensions;
using painternya.Models;
using DrawingContext = painternya.Models.DrawingContext;

namespace painternya.Services
{
    public class TileManager
    {
        // TODO - make this configurable
        private Tile[,] _tiles;
        private readonly OrderedDictionary _loadedTiles = new();
        private readonly int _maxLoadedTiles;
        
        public static int TileSize = 128;
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

        public void ClearAllTiles()
        {
            foreach (var tile in _loadedTiles.Values)
            {
                ((Tile)tile).Bitmap.Clear(Colors.Transparent);
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
        
        public Tile[,] GetAllTiles()
        {
            return _tiles;
        }
        
        public void InitializeTiles(int totalWidth, int totalHeight)
        {
            int tilesInWidth = (int)Math.Ceiling((double)totalWidth / TileSize);
            int tilesInHeight = (int)Math.Ceiling((double)totalHeight / TileSize);

            _tiles = new Tile[tilesInWidth, tilesInHeight];

            for (var x = 0; x < tilesInWidth; x++)
            {
                for (var y = 0; y < tilesInHeight; y++)
                {
                    int tileWidth = (x == tilesInWidth - 1 && totalWidth % TileSize != 0) ? totalWidth % TileSize : TileSize;
                    int tileHeight = (y == tilesInHeight - 1 && totalHeight % TileSize != 0) ? totalHeight % TileSize : TileSize;

                    _tiles[x, y] = new Tile(tileWidth, tileHeight);
                    _tiles[x, y].X = x;
                    _tiles[x, y].Y = y;
                }
            }

        }
        
        public Tile GetTile(int x, int y)
        {
            if (_tiles[x, y] == null)
            {
                _tiles[x, y] = new Tile(TileSize, TileSize);
            }
            
            var tile = _tiles[x, y];
            var key = (x, y);
            
            if (!_loadedTiles.Contains(key))
            {
                _loadedTiles.Add(key, tile);
            }
            
            return tile;
        }
    }
}