using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Mediaplayer2.ViewModels;
using Mediaplayer2.Views;

namespace Mediaplayer2;

public partial class App : Application
{
    public static SettingsPageViewModel SettingsViewModel { get; private set; }
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        
        SettingsViewModel = new SettingsPageViewModel();
        //var settingsViewModel = new SettingsPageViewModel();
        var savedThemeName = SettingsViewModel.LoadSelectedThemeName();
        var savedTheme = SettingsViewModel.Presets.FirstOrDefault(t => t.Name == savedThemeName);
        
        if (savedTheme != null)
        {
            SettingsViewModel.ApplyTheme(savedTheme);
        }
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Helper.Services();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
                Icon = new WindowIcon("Assets/appIcon.ico")
            };
            //var vm = new MainWindowViewModel(desktop.MainWindow);
            //desktop.MainWindow.DataContext = vm;
        }

        base.OnFrameworkInitializationCompleted();
    }
}