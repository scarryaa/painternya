using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Xaml.Interactions.Custom;
using painternya.Interfaces;
using painternya.Models;
using ReactiveUI;

namespace painternya.ViewModels
{
    public class NewCanvasDialogViewModel : ViewModelBase // assuming you are using ReactiveUI or replace with INotifyPropertyChanged
    {
        private IDialogService _dialogService;
        private int _width;
        private int _height;
        public ICommand CreateCanvasCommand { get; }
        public int Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }

        public int Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        public NewCanvasDialogViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            CreateCanvasCommand = ReactiveCommand.Create(CreateCanvas);
        }

        private void CreateCanvas()
        {
            var result = new CanvasCreationResult
            {
                Width = Width,
                Height = Height,
                Success = true
            };

            _dialogService.SetDialogResult(this, result);
            _dialogService.CloseDialog(this); 
        }
    }
}