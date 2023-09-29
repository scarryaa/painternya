using System;
using System.Collections.ObjectModel;
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
    private ImageTabViewModel? _activeImageTab;
    
    public ImageTabViewModel ActiveImageTab
    {
        get => _activeImageTab ??= ImageTabs[0];
        set => this.RaiseAndSetIfChanged(ref _activeImageTab, value);
    }
    
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
        _dialogService = dialogService;
        SelectToolCommand = ReactiveCommand.Create<string>(tool => ActiveImageTab.CanvasViewModel?.SelectTool(tool));
        
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
    }
    
    public void AddTab()
    {
        ImageTabs.Add(new ImageTabViewModel { Title = "New Tab" });
    }

    public void RemoveTab(ImageTabViewModel imageTab)
    {
        ImageTabs.Remove(imageTab);
    }
    
    private async Task ShowNewCanvasDialogAsync()
    {
        var dialogViewModel = new NewCanvasDialogViewModel(_dialogService);
        var result = await _dialogService.ShowDialog<CanvasCreationResult>(dialogViewModel);

        if (result is { Success: true })
        {
            var newLayerManager = new LayerManager(result.Width, result.Height);
            var newCanvasVm = new CanvasViewModel(newLayerManager, result.Width, result.Height);
            var newTabVm = new ImageTabViewModel { Title = "New Tab", CanvasViewModel = newCanvasVm };
            
            ImageTabs.Add(newTabVm);
            ActiveImageTab = newTabVm;
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