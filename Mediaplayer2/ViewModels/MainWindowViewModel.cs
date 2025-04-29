using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.Controls;
using Mediaplayer2.Views;
using ReactiveUI;

namespace Mediaplayer2.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    
    //public ObservableCollection<MainPageViewModel> Main { get; } = new();

    //private ReactiveObject? _currentView;

    /*private ReactiveObject? CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }*/
    
    public RoutingState Router { get; } = new RoutingState();
    
    public ReactiveCommand<Unit, IRoutableViewModel> ToMusicPageCommand { get; }

    public MainWindowViewModel()
    {
        
        //ToMainPageCommand = ReactiveCommand.Create(ShowMainPage);
        //ToMusicPageCommand = ReactiveCommand.Create(ShowMusicPage);
        //CurrentView = new MainPageViewModel();
        ToMusicPageCommand = ReactiveCommand.CreateFromObservable(Music);
        ToMusicPageCommand.Execute().Subscribe();
    }
    
    //public ReactiveCommand<Unit, Unit> ToMainPageCommand { get; }
    
    //private void ShowMainPage() => CurrentView = new MainPageViewModel();
    
    //private void ShowMusicPage() => CurrentView = new MusicPageViewModel();
    
    private IObservable<IRoutableViewModel> Music() => Observable.FromAsync(async CancellationToken => Router.Navigate.Execute(new MusicPageViewModel(this))).ObserveOn(RxApp.MainThreadScheduler).SelectMany(x => x);
}