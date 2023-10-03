using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
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
            
            Console.WriteLine("Active tab changed, '{0}' is now active", _activeImageTab.Title);
            
            // mark canvases as inactive
            foreach (var tab in ImageTabs)
            {
                if (tab.CanvasViewModel.IsActive) tab.LastActiveLayerId = tab.CanvasViewModel.DrawingContext.LayerManager.ActiveLayer.Id;
                tab.CanvasViewModel.IsActive = false;
            }
            _activeImageTab.CanvasViewModel.IsActive = true;
            _activeImageTab?.LoadResourcesCommand.Execute(null);
            
            Layer lastActiveLayer = _activeImageTab.CanvasViewModel.DrawingContext.LayerManager.FindLayerById(_activeImageTab.LastActiveLayerId);
            if (lastActiveLayer != null)
            {
                _activeImageTab.LayersPaneVm.ActiveLayer = _activeImageTab.LayersPaneVm.Layers.FirstOrDefault(layer => layer.Layer.Id == lastActiveLayer.Id);
                _activeImageTab.CanvasViewModel.DrawingContext.LayerManager.SetActiveLayer(lastActiveLayer);
            }
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
        _toolManager.SelectTool("Pencil");
        
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
            foreach (var tab in ImageTabs)
            {
                if (tab.CanvasViewModel.IsActive) tab.LastActiveLayerId = tab.CanvasViewModel.DrawingContext.LayerManager.ActiveLayer.Id;
                tab.CanvasViewModel.IsActive = false;
            }
            
            ActiveImageTab = newTabVm;
        }
    }
    
    private async Task New()
    {
        await ShowNewCanvasDialogAsync();
    }
    
    private async void Open()
    {
        var openFileDialog = new OpenFileDialog
        {
            Title = "Open Image",
            Filters = new List<FileDialogFilter>
            {
                new()
                {
                    Name = "Image Files",
                    Extensions = new List<string> { "png", "jpg", "jpeg", "bmp" }
                },
                new()
                {
                    Name = "All Files",
                    Extensions = new List<string> { "*" }
                }
            }
        };
        
        var window = App.Current.MainWindowInstance;
        string[] fileNames = await openFileDialog.ShowAsync(window);
        
        if (fileNames == null || fileNames.Length == 0)
        {
            return;
        }
        
        var filePath = fileNames[0];
        try
        {
            var writeableBitmap = LoadImageIntoWriteableBitmap(filePath);
            AddImageTab(writeableBitmap);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error opening file: " + ex.Message);
        }
    }

    private WriteableBitmap LoadImageIntoWriteableBitmap(string filePath)
    {
        using var originalImage = SkiaSharp.SKBitmap.Decode(filePath);
        
        var writeableBitmap = new WriteableBitmap(
            new PixelSize(originalImage.Width, originalImage.Height),
            new Vector(96, 96),
            PixelFormat.Bgra8888,
            AlphaFormat.Premul);
        
        using (var fb = writeableBitmap.Lock())
        {
            for (int y = 0; y < originalImage.Height; y++)
            {
                for (int x = 0; x < originalImage.Width; x++)
                {
                    var color = originalImage.GetPixel(x, y);

                    var index = y * fb.RowBytes + x * 4;
                        
                    Marshal.WriteInt32(fb.Address, index, color.Alpha << 24 | color.Red << 16 | color.Green << 8 | color.Blue);
                }
            }
        }
    
        return writeableBitmap;
    }
    
    private void AddImageTab(WriteableBitmap bitmap)
    {
        var newLayerManager = new LayerManager(bitmap.PixelSize.Width, bitmap.PixelSize.Height);
        var newCanvasVm =
            new CanvasViewModel(_toolManager, newLayerManager, bitmap.PixelSize.Width, bitmap.PixelSize.Height);
        
        newCanvasVm.SetBitmap(bitmap);

        var newTabVm = new ImageTabViewModel(CloseCommand, new LayersPaneViewModel(newLayerManager))
        {
            Title = "New Tab " + ImageTabs.Count,
            CanvasViewModel = newCanvasVm
        };

        ImageTabs.Add(newTabVm);
        foreach (var tab in ImageTabs)
        {
            if (tab.CanvasViewModel.IsActive) tab.LastActiveLayerId = tab.CanvasViewModel.DrawingContext.LayerManager.ActiveLayer.Id;
            tab.CanvasViewModel.IsActive = false;
        }

        ActiveImageTab = newTabVm;
    }

    
    private void Save()
    {

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