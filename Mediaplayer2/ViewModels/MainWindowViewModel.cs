using System.Collections.ObjectModel;
using System.Reactive;
using Mediaplayer2.Views;
using ReactiveUI;

namespace Mediaplayer2.ViewModels;

public class MainWindowViewModel : ReactiveObject
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    
    public ObservableCollection<MainPageViewModel> Main { get; } = new();

    private ReactiveObject _currentView;

    private ReactiveObject CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }

    public MainWindowViewModel()
    {
        CurrentView = new MainPageViewModel();
        ToMainPageCommand = ReactiveCommand.Create(() => CurrentView = new MainPageViewModel());
        ToMusicPageCommand = ReactiveCommand.Create(() => CurrentView = new MusicPageViewModel());
    }
    
    public ReactiveCommand<Unit, ReactiveObject> ToMainPageCommand { get; }
    
    public ReactiveCommand<Unit, ReactiveObject> ToMusicPageCommand { get; }
}