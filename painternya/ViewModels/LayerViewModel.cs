using System.Windows.Input;
using painternya.Models;
using painternya.Services;
using ReactiveUI;

namespace painternya.ViewModels;

public class LayerViewModel : ViewModelBase
{
    private Layer _layer;
    
    public Layer Layer
    {
        get => _layer;
        set => this.RaiseAndSetIfChanged(ref _layer, value);
    }

    public ICommand ToggleVisibilityCommand { get; }

    public LayerViewModel(Layer layer)
    {
        _layer = layer;
        ToggleVisibilityCommand = ReactiveCommand.Create<LayerViewModel>(ToggleVisibility);
    }

    private void ToggleVisibility(LayerViewModel vm)
    {
        // We do not need to toggle the visibility of the active layer, since it
        // is handled by the binding in the LayersPaneView.
        MessagingService.Instance.Publish(MessageType.LayerVisibilityChanged, Layer.IsVisible);
    }
}