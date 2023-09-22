using System.Windows.Input;
using painternya.Interfaces;
using painternya.Models;
using ReactiveUI;

namespace painternya.ViewModels
{
    public class NewCanvasDialogViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private int _width = 500;
        private int _height = 500;
        public ICommand CreateCanvasCommand { get; }
        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                this.RaisePropertyChanged();
            }
        }

        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                this.RaisePropertyChanged();
            }
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