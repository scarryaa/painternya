using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace painternya.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.AttachDevTools(new KeyGesture(Key.Y, KeyModifiers.Control));
    }
}