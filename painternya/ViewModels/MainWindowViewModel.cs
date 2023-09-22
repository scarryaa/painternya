using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using painternya.Interfaces;
using painternya.Models;
using painternya.Views;
using ReactiveUI;

namespace painternya.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private CanvasViewModel _canvasVM;

    public CanvasViewModel CanvasVM
    {
        get => _canvasVM;
        set => this.RaiseAndSetIfChanged(ref _canvasVM, value);
    }
    
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
    
    public ICommand SelectBrushCommand { get; set; }
    public ICommand SelectEraserCommand { get; set; }
    public ICommand SelectSelectionCommand { get; set; }
    public ICommand SelectMoveCommand { get; set; }
    public ICommand ScrolledCommand { get; set; }

    public MainWindowViewModel(IDialogService dialogService)
    {   
        _dialogService = dialogService;
        
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
        
        SelectBrushCommand = ReactiveCommand.Create(SelectBrush);
        SelectEraserCommand = ReactiveCommand.Create(SelectEraser);
        SelectSelectionCommand = ReactiveCommand.Create(SelectSelection);
        SelectMoveCommand = ReactiveCommand.Create(SelectMove);
        ScrolledCommand = ReactiveCommand.Create<object>(Scrolled);
    }
    
    public async Task ShowNewCanvasDialogAsync()
    {
        var dialogViewModel = new NewCanvasDialogViewModel(_dialogService);
        var result = await _dialogService.ShowDialog<CanvasCreationResult>(dialogViewModel);

        if (result != null && result.Success)
        {
            CanvasVM = new CanvasViewModel(result.Width, result.Height);
        }
    }
    
    private void Scrolled(object sender)
    {
        var scrollViewer = sender as ScrollViewer;
        if (scrollViewer == null) return;

        if (CanvasVM == null) return;
        
        CanvasVM.HorizontalOffset = scrollViewer.Offset.X;
        CanvasVM.VerticalOffset = scrollViewer.Offset.Y;
        CanvasVM.UpdateTileVisibilities();
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
    
    private void SelectBrush()
    {
        throw new System.NotImplementedException();
    }
    
    private void SelectEraser()
    {
        throw new System.NotImplementedException();
    }
    
    private void SelectSelection()
    {
        throw new System.NotImplementedException();
    }
    
    private void SelectMove()
    {
        throw new System.NotImplementedException();
    }
}