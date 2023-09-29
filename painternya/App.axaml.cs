using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using painternya.Interfaces;
using painternya.Services;
using painternya.ViewModels;
using painternya.Views;

namespace painternya;

public partial class App : Application
{
    public new static App Current => (App)Application.Current;
    
    public Window MainWindowInstance { get; set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var dialogService = Program.ServiceProvider.GetService<IDialogService>();
            
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(dialogService)
            };
            
            Current.MainWindowInstance = desktop.MainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}