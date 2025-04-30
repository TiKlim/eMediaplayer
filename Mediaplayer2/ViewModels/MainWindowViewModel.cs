using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.Controls;
using Mediaplayer2.Views;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public string Greeting { get; } = "Welcome to Avalonia!";
    
    public RoutingState Router { get; } = new RoutingState();
    
    public IScreen HostScreen { get; }
    
    public ReactiveCommand<Unit, Unit> ToMusicPageCommand { get; }

    public MainWindowViewModel()
    {
        HostScreen = Locator.Current.GetService<IScreen>()!;
        Debug.WriteLine("Навигация прошла");
        Router.Navigate.Execute(new MainPageViewModel(this));
        //ToMusicPageCommand = ReactiveCommand.CreateFromObservable(Music);
        ToMusicPageCommand = ReactiveCommand.Create(() =>
        {
            Router.Navigate.Execute(new MusicPageViewModel(this));
        });
    }
    
    /*private IObservable<IRoutableViewModel> Music() => Observable.FromAsync(async cancellationToken =>
    {
        var musicPageViewModel = new MusicPageViewModel(this);
        await Router.Navigate.Execute(musicPageViewModel);
        return musicPageViewModel;
    });*/
}