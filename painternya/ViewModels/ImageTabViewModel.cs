using System;
using System.Diagnostics;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Xaml.Interactions.Custom;
using painternya.Models;
using painternya.Services;
using ReactiveUI;

namespace painternya.ViewModels;

public class ImageTabViewModel : ViewModelBase
{
    private ScrollViewer? _scrollViewer;
    public string LastActiveLayerId { get; set; }
    private Bitmap? _thumbnail => CanvasViewModel.Thumbnail;
    private LayersPaneViewModel? _layersPaneVm;
    private double _zoom = 1.0;
    private double _translateX;
    private double _translateY;
    private Vector _viewport;

    public Bitmap? Thumbnail
    {
        get => _thumbnail;
    }
    public ICommand UnloadResourcesCommand { get; }
    public ICommand LoadResourcesCommand { get; }
    public CanvasViewModel CanvasViewModel { get; set; }
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
        set
        {
            this.RaiseAndSetIfChanged(ref _zoom, value);
            CanvasViewModel.Zoom = value;
        }
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
    public ICommand CloseTabCommand { get; }

    public ImageTabViewModel(ICommand closeTabCommand, LayersPaneViewModel layersPaneVm)
    {
        LayersPaneVm = layersPaneVm;
        CloseTabCommand = closeTabCommand;
        UnloadResourcesCommand = ReactiveCommand.Create(UnloadResources);
        LoadResourcesCommand = ReactiveCommand.Create(LoadResources);
        
        ScrolledCommand = ReactiveCommand.Create<object>(Scrolled);
        ZoomCommand = ReactiveCommand.Create<PointerWheelChangedArgs>(ZoomAtPoint);
    }
    
    public void ZoomAtPoint(PointerWheelChangedArgs args)
    {
        if (CanvasViewModel.IsActive == false) return;
        
        var oldZoom = Zoom;
        double zoomFactor = args.Delta > 0 ? 1.01 : 0.99;

        Zoom *= zoomFactor;

        if (Zoom < 0.1) Zoom = 0.1;
        if (Zoom > 10) Zoom = 10;

        var preZoomMouseContentX = (args.Position.X + TranslateX) / oldZoom;
        var preZoomMouseContentY = (args.Position.Y + TranslateY) / oldZoom;

        var postZoomMouseContentX = preZoomMouseContentX * Zoom;
        var postZoomMouseContentY = preZoomMouseContentY * Zoom;

        TranslateX = postZoomMouseContentX - args.Position.X;
        TranslateY = postZoomMouseContentY - args.Position.Y;

        CanvasViewModel.Offset = new Vector(TranslateX, TranslateY);
        CanvasViewModel.DrawingContext.Zoom = Zoom;
        CanvasViewModel.DrawingContext.Viewport = Viewport;
    }
    
    private void Scrolled(object sender)
    {
        if (CanvasViewModel.IsActive == false) return;

        _scrollViewer = sender as ScrollViewer;
        if (_scrollViewer == null) return;

        if (CanvasViewModel == null) return;
        
        CanvasViewModel.OffsetX = _scrollViewer.Offset.X;
        CanvasViewModel.OffsetY = _scrollViewer.Offset.Y;
        CanvasViewModel.Offset = new Vector(_scrollViewer.Offset.X, _scrollViewer.Offset.Y);
        
        TranslateX = _scrollViewer.Offset.X;
        TranslateY = _scrollViewer.Offset.Y;
    }
    
    
    private void UnloadResources()
    {
        CanvasViewModel.Dispose();
        LayersPaneVm = null;
    }

    private void LoadResources()
    {
    }
}
