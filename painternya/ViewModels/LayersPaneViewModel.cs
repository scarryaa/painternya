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
        private Layer _activeLayer;
        public ObservableCollection<Layer> Layers { get; set; }
        public Layer ActiveLayer
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
                layerManager.AddLayer(new Layer($"Layer {Layers.Count + 1}", 500, 500)));
            RemoveLayerCommand = ReactiveCommand.Create(() =>
            {
                var oldActiveLayer = ActiveLayer;
                layerManager.SetActiveLayer(0);
                ActiveLayer = layerManager.ActiveLayer;
                layerManager.Layers.Remove(oldActiveLayer);
            });
            SelectionChangedCommand = ReactiveCommand.Create<Layer>(layerManager.SetActiveLayer);
        }
    }
}