using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Mail;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.Controls;
using Mediaplayer2.Views;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class MainWindowViewModel : ReactiveObject //ViewModelBase, IScreen
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    private string _selectedClasses;

    private string _background;

    private object _currentView;

    public string SelectedClasses
    {
        get => _selectedClasses;
        set => this.RaiseAndSetIfChanged(ref _selectedClasses, value);
    }

    public string Background
    {
        get => _background;
        set => this.RaiseAndSetIfChanged(ref _background, value);
    }

    public object CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }
    
    public ReactiveCommand<Unit, Unit> ToHomePageCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ToMusicPageCommand { get; }

    public MainWindowViewModel()
    {
        Background = "Transparent";
        CurrentView = new MainPageView();
        ToHomePageCommand = ReactiveCommand.Create(HomePage);
        ToMusicPageCommand = ReactiveCommand.Create(MusicPage);

        if ((CurrentView = new MainPageView()) != null)
        {
            Background = "#f7d2d3";
            SelectedClasses = "SelectedPage";
        }
        else if ((CurrentView = new MusicPageView()) != null)
        {
            Background = "#f7d2d3";
            SelectedClasses = "SelectedPage";
        }
    }
    
    private void HomePage()
    {
        CurrentView = new MainPageView();
    }

    private void MusicPage()
    {
        CurrentView = new MusicPageView();
    }
}