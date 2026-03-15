using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Mediaplayer2.Services;
using Mediaplayer2.ViewModels;
using Mediaplayer2.Views;
using Splat;

namespace Mediaplayer2;

public partial class App : Application
{
    public static MainPageViewModel MainViewModel { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        MainViewModel = new MainPageViewModel();
        
        Locator.CurrentMutable.RegisterConstant(new ThemeService(), typeof(IThemeService));
        Locator.CurrentMutable.RegisterConstant(new LanguageService(), typeof(ILanguageService));
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Helper.Services();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
                Icon = new WindowIcon("Assets/appIcon2.ico")
            };
            //var vm = new MainWindowViewModel(desktop.MainWindow);
            //desktop.MainWindow.DataContext = vm;
        }

        base.OnFrameworkInitializationCompleted();
    }
}