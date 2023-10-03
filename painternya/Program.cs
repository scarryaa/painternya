using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Microsoft.Extensions.DependencyInjection;
using painternya.Interfaces;
using painternya.Services;
using painternya.Views;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;

namespace painternya;

class Program
{
    public static IServiceProvider ServiceProvider { get; private set; }
    
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        IconProvider.Current
            .Register<FontAwesomeIconProvider>();
        
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IDialogService, DialogService>();
        
        ServiceProvider = serviceCollection.BuildServiceProvider();

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI()
            .UseSkia();
    }

}