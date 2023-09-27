using System.Collections.Generic;
using painternya.Models;

namespace painternya.Services
{
    public class LayerManager
    {
        private readonly List<Layer> _layers = new();
        public IReadOnlyList<Layer> Layers => _layers.AsReadOnly();
        public Layer ActiveLayer { get; set; }
        public int TotalWidth { get; set; }
        public int TotalHeight { get; set; }
        
        public LayerManager(int totalWidth, int totalHeight)
        {
            TotalWidth = totalWidth;
            TotalHeight = totalHeight;
            _layers.Add(new Layer(TotalWidth, TotalHeight)
            {
                Name = "Layer 1",
                IsVisible = true
            });
            
            ActiveLayer = _layers[0];
        }
        
        public void AddLayer(Layer layer)
        {
            _layers.Add(layer);
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
        
        public void SetActiveLayer(Layer layer)
        {
            ActiveLayer = layer;
        }
        
        public void SetActiveLayer(int index)
        {
            if (index < 0 || index >= _layers.Count)
                return;

            ActiveLayer = _layers[index];
        }
        
        public List<Layer> GetAllLayers()
        {
            return _layers;
        }
    }
}