using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using painternya.Controls;
using painternya.Interfaces;
using painternya.Models;
using painternya.Services;
using ReactiveUI;

namespace painternya.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private CanvasViewModel? _canvasVm;
    private LayersPaneViewModel? _layersPaneVm;
    private double _zoom = 1.0;
    private double _translateX;
    private double _translateY;
    private Vector _viewport;

    public CanvasViewModel? CanvasVm
    {
        get => _canvasVm;
        set => this.RaiseAndSetIfChanged(ref _canvasVm, value);
    }
    
    public LayersPaneViewModel? LayersPaneVm
    {
        get => _layersPaneVm;
        set => this.RaiseAndSetIfChanged(ref _layersPaneVm, value);
    }
    
    public Vector Viewport
    {
        get
        {
            if (CanvasVm == null) return new Vector(0, 0);
            return new Vector(CanvasVm.CanvasWidth * Zoom, CanvasVm.CanvasHeight * Zoom);
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
    
    public ICommand SelectToolCommand { get; set; }
    public ICommand NewCommand { get; set; }
    public ICommand OpenCommand { get; set; }
    public ICommand SaveCommand { get; set; }
    public ICommand SaveAsCommand { get; set; }
    public ICommand ExitCommand { get; set; }
    public ICommand UndoCommand { get; set; }
    public ICommand RedoCommand { get; set; }
    public ICommand CutCommand { get; set; }
    public ICommand CopyCommand { get; set; }
    public ICommand PasteCommand { get; set; }
    public ICommand DeleteCommand { get; set; }
    public ICommand SelectAllCommand { get; set; }
    
    public ICommand ScrolledCommand { get; set; }
    public ICommand ZoomCommand { get; set; }

    public MainWindowViewModel(IDialogService dialogService)
    {
        _dialogService = dialogService;
        SelectToolCommand = ReactiveCommand.Create<string>(tool => CanvasVm?.SelectTool(tool));
        
        NewCommand = ReactiveCommand.Create(New);
        OpenCommand = ReactiveCommand.Create(Open);
        SaveCommand = ReactiveCommand.Create(Save);
        SaveAsCommand = ReactiveCommand.Create(SaveAs);
        ExitCommand = ReactiveCommand.Create(Exit);
        UndoCommand = ReactiveCommand.Create(Undo);
        RedoCommand = ReactiveCommand.Create(Redo);
        CutCommand = ReactiveCommand.Create(Cut);
        CopyCommand = ReactiveCommand.Create(Copy);
        PasteCommand = ReactiveCommand.Create(Paste);
        DeleteCommand = ReactiveCommand.Create(Delete);
        SelectAllCommand = ReactiveCommand.Create(SelectAll);
        
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
        CanvasVm.Offset = new Vector(TranslateX, TranslateY);
        CanvasVm.DrawingContext.Zoom = Zoom;
        CanvasVm.DrawingContext.Viewport = Viewport;
    }

    
    private async Task ShowNewCanvasDialogAsync()
    {
        var dialogViewModel = new NewCanvasDialogViewModel(_dialogService);
        var result = await _dialogService.ShowDialog<CanvasCreationResult>(dialogViewModel);

        if (result is { Success: true })
        {
            var newLayerManager = new LayerManager(result.Width, result.Height);
            CanvasVm = new CanvasViewModel(newLayerManager, result.Width, result.Height);
            LayersPaneVm = new LayersPaneViewModel(newLayerManager);
        }
    }
    
    private void Scrolled(object sender)
    {
        var scrollViewer = sender as ScrollViewer;
        if (scrollViewer == null) return;

        if (CanvasVm == null) return;
        
        CanvasVm.OffsetX = scrollViewer.Offset.X;
        CanvasVm.OffsetY = scrollViewer.Offset.Y;
        // CanvasVm.TileManager.UpdateTileVisibilities();
    }

    private async Task New()
    {
        await ShowNewCanvasDialogAsync();
    }
    
    private void Open()
    {
        throw new System.NotImplementedException();
    }
    
    private void Save()
    {
        throw new System.NotImplementedException();
    }
    
    private void SaveAs()
    {
        throw new System.NotImplementedException();
    }
    
    private void Exit()
    {
        throw new System.NotImplementedException();
    }
    
    private void Undo()
    {
        throw new System.NotImplementedException();
    }
    
    private void Redo()
    {
        throw new System.NotImplementedException();
    }
    
    private void Cut()
    {
        throw new System.NotImplementedException();
    }
    
    private void Copy()
    {
        throw new System.NotImplementedException();
    }
    
    private void Paste()
    {
        throw new System.NotImplementedException();
    }
    
    private void Delete()
    {
        throw new System.NotImplementedException();
    }
    
    private void SelectAll()
    {
        throw new System.NotImplementedException();
    }
}