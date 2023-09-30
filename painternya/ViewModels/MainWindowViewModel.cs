using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using painternya.Controls;
using painternya.Interfaces;
using painternya.Models;
using painternya.Services;
using painternya.Tools;
using ReactiveUI;

namespace painternya.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDialogService _dialogService;
    private readonly ToolManager _toolManager;
    private ImageTabViewModel? _activeImageTab = null;
    private ITool _pencil;
    private ITool _eraser;
    private ITool _brush;
    public ITool Pencil => _pencil;
    public ITool Eraser => _eraser;
    public ITool Brush => _brush;
    
    public ToolManager ToolManager => _toolManager;
    public ImageTabViewModel ActiveImageTab
    {
        get => _activeImageTab;
        set
        {
            this.RaiseAndSetIfChanged(ref _activeImageTab, value);
            
            // mark canvases as inactive
            foreach (var tab in ImageTabs)
            {
                tab.CanvasViewModel.IsActive = false;
            }
            _activeImageTab.CanvasViewModel.IsActive = true;
            _activeImageTab?.LoadResourcesCommand.Execute(null);
            _activeImageTab.CanvasViewModel.DrawingContext.LayerManager.ActiveLayer = _activeImageTab.CanvasViewModel.DrawingContext.LayerManager.Layers[0];
        }
    }
    
    public List<ITool> Tools => new() { Pencil, Eraser, Brush };
    
    public ICommand CloseCommand { get; set; }
    public ICommand TabControlSelectionChangedCommand { get; set; }
    public ObservableCollection<ImageTabViewModel> ImageTabs { get; } = new();
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

    public MainWindowViewModel() {}
    
    public MainWindowViewModel(IDialogService dialogService)
    {
        _toolManager = new ToolManager();
        _pencil = new PencilTool(_toolManager.GlobalCurrentToolSize);
        _eraser = new EraserTool(_toolManager.GlobalCurrentToolSize);
        _brush = new BrushTool(_toolManager.GlobalCurrentToolSize);
        
        _dialogService = dialogService;
        SelectToolCommand = ReactiveCommand.Create<string>(tool => _toolManager.SelectTool(tool));
        
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
        
        CloseCommand = ReactiveCommand.Create<ImageTabViewModel>(RemoveTab);
        TabControlSelectionChangedCommand = ReactiveCommand.Create<TabControl>(tabControl =>
        {
            if (tabControl.SelectedItem is ImageTabViewModel imageTab)
            {
                ActiveImageTab = imageTab;
            }
        });
    }
    
    public void AddTab()
    {
        ImageTabs.Add(new ImageTabViewModel(CloseCommand, new LayersPaneViewModel(new LayerManager(500, 500)))
        {
            Title = "New Tab " + ImageTabs.Count
        });
    }

    public void RemoveTab(ImageTabViewModel imageTab)
    {
        imageTab.LayersPaneVm = null;
        imageTab.CanvasViewModel.Dispose();
        ImageTabs.Remove(imageTab);
    }
    
    private async Task ShowNewCanvasDialogAsync()
    {
        var dialogViewModel = new NewCanvasDialogViewModel(_dialogService);
        var result = await _dialogService.ShowDialog<CanvasCreationResult>(dialogViewModel);

        if (result is { Success: true })
        {
            var newLayerManager = new LayerManager(result.Width, result.Height);
            var newCanvasVm = new CanvasViewModel(_toolManager, newLayerManager, result.Width, result.Height);
            var newTabVm = new ImageTabViewModel(CloseCommand, new LayersPaneViewModel(newLayerManager))
            {
                Title = "New Tab " + ImageTabs.Count,
                CanvasViewModel = newCanvasVm
            };
            
            ImageTabs.Add(newTabVm);
            ActiveImageTab = newTabVm;
            
            // mark canvases as inactive
            foreach (var tab in ImageTabs)
            {
                tab.CanvasViewModel.IsActive = false;
            }
            
            // mark new canvas as active
            newCanvasVm.IsActive = true;
        }
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