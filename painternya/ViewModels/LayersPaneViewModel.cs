using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using painternya.Models;
using painternya.Services;
using ReactiveUI;

namespace painternya.ViewModels
{
    public class LayersPaneViewModel : ViewModelBase
    {
        private int _layerCount = 1;
        private Layer? _activeLayer;
        public ObservableCollection<Layer> Layers { get; set; }
        public Layer? ActiveLayer
        {
            get => _activeLayer;
            set => this.RaiseAndSetIfChanged(ref _activeLayer, value);
        }
        public ICommand AddLayerCommand { get; set; }
        public ICommand RemoveLayerCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }

        public LayersPaneViewModel(LayerManager layerManager)
        {
            Layers = layerManager.Layers;
            ActiveLayer = layerManager.ActiveLayer;

            AddLayerCommand = ReactiveCommand.Create(() =>
            {
                layerManager.AddLayer(new Layer($"Layer {++_layerCount}", 500, 500));
                ActiveLayer = layerManager.ActiveLayer;
            });
            
            RemoveLayerCommand = ReactiveCommand.Create(() =>
            {
                var oldActiveLayer = ActiveLayer;
                var oldIndex = layerManager.Layers.IndexOf(oldActiveLayer);
                if (layerManager.Layers.Count > 1 && oldIndex != 0)
                {
                    layerManager.SetActiveLayer(oldIndex - 1);
                } else if (oldIndex == 0 && layerManager.Layers.Count > 1)
                {
                    layerManager.SetActiveLayer(1);
                }
                else
                {
                    layerManager.SetActiveLayer(null);
                }
                
                ActiveLayer = layerManager.ActiveLayer;
                if (oldActiveLayer != null) layerManager.Layers.Remove(oldActiveLayer);
                
                MessagingService.Instance.Publish(MessageType.LayerRemoved);
            });
            SelectionChangedCommand = ReactiveCommand.Create<Layer>(layerManager.SetActiveLayer);
        }
    }
}