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

public class MainWindowViewModel : ReactiveObject
{
    public string Greeting { get; } = "Welcome to Avalonia!";

    private bool _isSelected;

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
    
    public ReactiveCommand<Unit, Unit> ToVideoPageCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ToPlaylistPageCommand { get; }
    
    public ReactiveCommand<Unit, Unit> ToSettingsPageCommand { get; }
    
    //public ReactiveCommand<Unit, Unit> ToAudioEditPageCommand { get; }

    public MainWindowViewModel()
    {
        Background = "Transparent";
        CurrentView = new MainPageView();
        ToHomePageCommand = ReactiveCommand.Create(HomePage);
        ToMusicPageCommand = ReactiveCommand.Create(MusicPage);
        ToVideoPageCommand = ReactiveCommand.Create(VideoPage);
        ToPlaylistPageCommand = ReactiveCommand.Create(PlaylistPage);
        ToSettingsPageCommand = ReactiveCommand.Create(SettingsPage);
        //ToAudioEditPageCommand = ReactiveCommand.Create(AudioEditPage);

        if (_isSelected) 
        {
            Background = "#f7d2d3";
            SelectedClasses = "SelectedPage";
        }
        else
        {
            Background = "Transparent";
            SelectedClasses = "LeftPanel";
        }
    }
    
    private void HomePage()
    {
        CurrentView = new MainPageView();
        _isSelected = true; //???
    }

    private void MusicPage()
    {
        CurrentView = new MusicPageView();
    }
    
    private void VideoPage()
    {
        CurrentView = new VideoPageView();
    }
    
    private void PlaylistPage()
    {
        CurrentView = new PlaylistPageView();
    }
    
    private void SettingsPage()
    {
        CurrentView = new SettingsPageView();
    }

    /*private void AudioEditPage()
    {
        CurrentView = new EditAudioView();
    }*/
}