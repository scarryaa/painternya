using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using DynamicData.Binding;
using painternya.Extensions;
using painternya.Models;
using Bitmap = System.Drawing.Bitmap;

namespace painternya.Services
{
    public class LayerManager
    {
        private Layer? _activeLayer;
        private ObservableCollection<Layer> _layers = new();
        public ObservableCollection<Layer> Layers => _layers;
        public Layer? ActiveLayer { get; set; }
        public Layer PreviewLayer { get; private set; }
        public int TotalWidth { get; set; }
        public int TotalHeight { get; set; }
        
        public LayerManager(int totalWidth, int totalHeight)
        {
            TotalWidth = totalWidth;
            TotalHeight = totalHeight;
            _layers.Add(new Layer("Layer 1",TotalWidth, TotalHeight));
            
            ActiveLayer = _layers.Count > 0 ? _layers[0] : null;
            ActiveLayer.TileManager.ClearAllTiles(Colors.White);
            PreviewLayer = new Layer("Preview", TotalWidth, TotalHeight);
        }
        
        public void ClearPreviewLayer()
        {
            PreviewLayer.TileManager.ClearAllTiles(Colors.Transparent);
        }
        
        public void AddLayer(Layer layer)
        {
            _layers.Add(layer);
            ActiveLayer = layer;
        }
        
        public void RemoveLayer(Layer layer)
        {
            _layers.Remove(layer);
        }
        
        public void RemoveLayerAt(int index)
        {
            _layers.RemoveAt(index);
        }
        
        public void MoveLayer(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= _layers.Count || newIndex < 0 || newIndex >= _layers.Count)
                return;

            var layer = _layers[oldIndex];
            _layers.RemoveAt(oldIndex);
            _layers.Insert(newIndex, layer);
        }
        
        public void ClearLayers()
        {
            _layers.Clear();
        }
        
        public void SetActiveLayer(Layer? layer)
        {
            ActiveLayer = layer;
        }
        
        public void SetActiveLayer(int index)
        {
            if (index < 0 || index >= _layers.Count)
                return;

            ActiveLayer = _layers[index];
        }
        
        public ObservableCollection<Layer> GetAllLayers()
        {
            return _layers;
        }
        
        public void MergePreviewToActiveLayer()
        {
            foreach (var tile in PreviewLayer.TileManager.GetAllTiles())
            {
                _layers[0].TileManager.GetTile(tile.Value.X, tile.Value.Y).Bitmap.Clone(tile.Value.Bitmap);
            }
        }
        
        public Layer FindLayerById(string id)
        {
            return Layers.FirstOrDefault(layer => layer.Id == id);
        }
    }
}