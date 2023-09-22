using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using painternya.Interfaces;
using painternya.Models;
using painternya.ViewModels;

namespace painternya.Views
{
    public partial class NewCanvasDialog : Window, IHasDialogResult
    {
        public object DialogResultValue { get; set; }

        public NewCanvasDialog()
        {
            InitializeComponent();
        }
    }
}