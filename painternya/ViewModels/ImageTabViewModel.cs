using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using painternya.Models;
using painternya.Services;
using ReactiveUI;

namespace painternya.ViewModels;

public class ImageTabViewModel : ViewModelBase
{
    private LayersPaneViewModel? _layersPaneVm;
    private double _zoom = 1.0;
    private double _translateX;
    private double _translateY;
    private Vector _viewport;
    
    public CanvasViewModel CanvasViewModel { get; set; } = new();
    public LayersPaneViewModel LayersPaneVm
    {
        get => _layersPaneVm ??= new LayersPaneViewModel(CanvasViewModel.DrawingContext.LayerManager);
        set => this.RaiseAndSetIfChanged(ref _layersPaneVm, value);
    }
    
    public string Title { get; set; }
    
    public Vector Viewport
    {
        get
        {
            if (CanvasViewModel == null) return new Vector(0, 0);
            return new Vector(CanvasViewModel.CanvasWidth * Zoom, CanvasViewModel.CanvasHeight * Zoom);
        }
        set => this.RaiseAndSetIfChanged(ref _viewport, value);
    }
    
    public double Zoom
    {
        get => _zoom;
        set => this.RaiseAndSetIfChanged(ref _zoom, value);
    }
    
    public double TranslateX
    {
        get => _translateX;
        set => this.RaiseAndSetIfChanged(ref _translateX, value);
    }
    
    public double TranslateY
    {
        get => _translateY;
        set => this.RaiseAndSetIfChanged(ref _translateY, value);
    }
    
    public ICommand ScrolledCommand { get; set; }
    public ICommand ZoomCommand { get; set; }

    public ImageTabViewModel()
    {
        ScrolledCommand = ReactiveCommand.Create<object>(Scrolled);
        ZoomCommand = ReactiveCommand.Create<PointerWheelChangedArgs>(ZoomAtPoint);
    }
    
    public void ZoomAtPoint(PointerWheelChangedArgs args)
    {
        double oldZoom = Zoom;
        
        double zoomFactor = args.Delta > 0 ? 1.01 : 0.99;

        Zoom *= zoomFactor;

        if (Zoom < 0.1) Zoom = 0.1;
        if (Zoom > 10) Zoom = 10;

        TranslateX = (args.Position.X + TranslateX) * zoomFactor - args.Position.X;
        TranslateY = (args.Position.Y + TranslateY) * zoomFactor - args.Position.Y;
        CanvasViewModel.Offset = new Vector(TranslateX, TranslateY);
        CanvasViewModel.DrawingContext.Zoom = Zoom;
        CanvasViewModel.DrawingContext.Viewport = Viewport;
    }
    
    private void Scrolled(object sender)
    {
        var scrollViewer = sender as ScrollViewer;
        if (scrollViewer == null) return;

        if (CanvasViewModel == null) return;
        
        CanvasViewModel.OffsetX = scrollViewer.Offset.X;
        CanvasViewModel.OffsetY = scrollViewer.Offset.Y;
        // CanvasVm.TileManager.UpdateTileVisibilities();
    }
}