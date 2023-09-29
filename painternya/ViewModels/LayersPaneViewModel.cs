using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using painternya.Models;
using painternya.Services;
using ReactiveUI;

namespace painternya.ViewModels;

public class LayersPaneViewModel : ViewModelBase
{
    private int _layerCount = 1;
    private LayerViewModel? _activeLayer;
    public ObservableCollection<LayerViewModel> Layers { get; set; }
    
    public LayerViewModel? ActiveLayer
    {
        get => _activeLayer;
        set => this.RaiseAndSetIfChanged(ref _activeLayer, value);
    }

    public ICommand AddLayerCommand { get; }
    public ICommand RemoveLayerCommand { get; }
    public ICommand SelectionChangedCommand { get; }

    public LayersPaneViewModel(LayerManager layerManager)
    {
        Layers = new ObservableCollection<LayerViewModel>(
            layerManager.Layers.Select(layer => new LayerViewModel(layer)));
        
        ActiveLayer = Layers.FirstOrDefault();

        AddLayerCommand = ReactiveCommand.Create(() =>
        {
            var newLayer = new Layer($"Layer {++_layerCount}", 500, 500);
            layerManager.AddLayer(newLayer);
            var newLayerViewModel = new LayerViewModel(newLayer);
            Layers.Add(newLayerViewModel);
            ActiveLayer = newLayerViewModel;
            
            MessagingService.Instance.Publish(MessageType.LayerAdded);
        });
        
        RemoveLayerCommand = ReactiveCommand.Create(() =>
        {
            var oldActiveLayer = ActiveLayer;
            if (oldActiveLayer != null)
            {
                var oldIndex = layerManager.Layers.IndexOf(oldActiveLayer.Layer);
                
                Layers.Remove(oldActiveLayer);
                layerManager.Layers.Remove(oldActiveLayer.Layer);
                
                if (layerManager.Layers.Count > 0)
                {
                    if (oldIndex > 0)
                    {
                        layerManager.SetActiveLayer(oldIndex - 1);
                        ActiveLayer = Layers[oldIndex - 1];
                    }
                    else
                    {
                        layerManager.SetActiveLayer(0);
                        ActiveLayer = Layers[0];
                    }
                }
                else
                {
                    layerManager.SetActiveLayer(null);
                    ActiveLayer = null;
                }
        
                MessagingService.Instance.Publish(MessageType.LayerRemoved);
            }
        });
        
        SelectionChangedCommand = ReactiveCommand.Create<LayerViewModel>(layerViewModel =>
        {
            layerManager.SetActiveLayer(layerViewModel.Layer);
            ActiveLayer = layerViewModel;
        });
    }
}