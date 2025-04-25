using System.Collections.ObjectModel;
using System.Reactive;
using Mediaplayer2.Views;
using ReactiveUI;

namespace Mediaplayer2.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    
    public ObservableCollection<MainPageViewModel> Main { get; } = new();

    private ReactiveObject? _currentView;

    private ReactiveObject? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public MainWindowViewModel()
    {
        
        //ToMainPageCommand = ReactiveCommand.Create(ShowMainPage);
        ToMusicPageCommand = ReactiveCommand.Create(ShowMusicPage);
        //CurrentView = new MainPageViewModel();
    }
    
    public ReactiveCommand<Unit, Unit> ToMainPageCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ToMusicPageCommand { get; }
    
    //private void ShowMainPage() => CurrentView = new MainPageViewModel();
    
    private void ShowMusicPage() => CurrentView = new MusicPageViewModel();
}