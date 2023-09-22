using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using painternya.Interfaces;
using painternya.Models;
using ReactiveUI;

namespace painternya.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private CanvasViewModel? _canvasVm;
    private double _zoom = 1.0;

    public CanvasViewModel? CanvasVm
    {
        get => _canvasVm;
        set => this.RaiseAndSetIfChanged(ref _canvasVm, value);
    }
    
    public double Zoom
    {
        get => _zoom;
        set => this.RaiseAndSetIfChanged(ref _zoom, value);
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
        ZoomCommand = ReactiveCommand.Create<PointerWheelChangedArgs>(HandleZoom);
    }

    private void HandleZoom(PointerWheelChangedArgs obj)
    {
        Point preZoomCursorPosRelativeToContent = obj.Position;

        double oldZoom = Zoom;
        Zoom = Math.Clamp(Zoom + obj.Delta / 10, 0.1, 10);

        Console.WriteLine(preZoomCursorPosRelativeToContent);
        Point postZoomCursorPosRelativeToContent = preZoomCursorPosRelativeToContent * (Zoom / oldZoom);
        CanvasVm.OffsetX += (postZoomCursorPosRelativeToContent.X - preZoomCursorPosRelativeToContent.X);
        CanvasVm.OffsetY += (postZoomCursorPosRelativeToContent.Y - preZoomCursorPosRelativeToContent.Y);

        CanvasVm.DrawingContext.Zoom = Zoom;
    }


    private async Task ShowNewCanvasDialogAsync()
    {
        var dialogViewModel = new NewCanvasDialogViewModel(_dialogService);
        var result = await _dialogService.ShowDialog<CanvasCreationResult>(dialogViewModel);

        if (result is { Success: true })
        {
            CanvasVm = new CanvasViewModel(result.Width, result.Height);
        }
    }
    
    private void Scrolled(object sender)
    {
        var scrollViewer = sender as ScrollViewer;
        if (scrollViewer == null) return;

        if (CanvasVm == null) return;
        
        CanvasVm.OffsetX = scrollViewer.Offset.X;
        CanvasVm.OffsetY = scrollViewer.Offset.Y;
        CanvasVm.DrawingContext.UpdateTileVisibilities();
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