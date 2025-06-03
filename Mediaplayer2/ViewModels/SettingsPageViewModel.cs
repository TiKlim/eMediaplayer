using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Mediaplayer2.Models;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class SettingsPageViewModel : ReactiveObject, IRoutableViewModel
{
    public ObservableCollection<Theme> Presets { get; }
    
    private Theme _selectedTheme;
    public Theme SelectedTheme
    {
        get => _selectedTheme;
        set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
    }
    
    public string Main { get; }
    
    public string PreMain { get; }
    
    public string? UrlPathSegment => "/settings";
    
    public IScreen HostScreen { get; }
    

    public SettingsPageViewModel()
    {
        
    }

    public SettingsPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        
        Main = "Настройки";
        PreMain = "Настрой под настроение!";
        
        Presets = new ObservableCollection<Theme>
        {
            new Theme
            {
                Name = "Клубника", 
                PrimaryColor = "#FF595E", 
                SecondaryColor = "#f5efef", 
                HomeButton = "Assets/HomeRed.png", 
                MusicButton = "Assets/MusicCollectionRed.png", 
                VideoButton = "Assets/VideoCollectionRed.png",
                PlaylistButton = "Assets/PlaylistRed.png",
                SettingsButton = "Assets/SettingsRed.png",
                PlayButton = "Assets/ButtonPlayRed.png",
                StopButton = "Assets/StopRed.png",
                BackTenButton = "Assets/BackTimeRed.png",
                ForeTenButton = "Assets/ForeTimeRed.png",
                BackwardButton = "Assets/BackwardRed.png",
                ForewardButton = "Assets/ForwardRed.png",
                EqualizerButton = "Assets/EqualizerRed.png",
                EditAudioButton = "Assets/EditAudioRed.png",
                EditVideoButton = "Assets/EditVideoRed.png",
                VolumeOnButton = "Assets/VolumeOnRed.png",
                VolumeOffButton = "Assets/VolumeOffRed.png",
                ListStopBack = "Assets/ListStopRed.png",
                MainBack = "Assets/MainPagePictureRed2.png",
                MusicBack = "Assets/MusicPagePictureRed2.png",
                VideoBack = "Assets/VideoPagePictureRed2.png",
                PlaylistBack = "Assets/PlaylistPagePictureRed2.png"
            },
            new Theme
            {
                Name = "Банан", 
                PrimaryColor = "#FFCA3A", 
                SecondaryColor = "#f5f3ed", 
                HomeButton = "Assets/HomeYello.png", 
                MusicButton = "Assets/MusicCollectionYello.png", 
                VideoButton = "Assets/VideoCollectionYello.png",
                PlaylistButton = "Assets/PlaylistYello.png",
                SettingsButton = "Assets/SettingsYello.png",
                PlayButton = "Assets/ButtonPlayYellow.png",
                StopButton = "Assets/StopYellow.png",
                BackTenButton = "Assets/BackTimeYellow.png",
                ForeTenButton = "Assets/ForeTimeYellow.png",
                BackwardButton = "Assets/BackwardYellow.png",
                ForewardButton = "Assets/ForwardYellow.png",
                EqualizerButton = "Assets/EqualizerYellow.png",
                EditAudioButton = "Assets/EditAudioYellow.png",
                EditVideoButton = "Assets/EditVideoYellow.png",
                VolumeOnButton = "Assets/VolumeOnYellow.png",
                VolumeOffButton = "Assets/VolumeOffYellow.png",
                ListStopBack = "Assets/ListStopYellow.png",
                MainBack = "Assets/MainPagePictureYellow2.png",
                MusicBack = "Assets/MusicPagePictureYellow2.png",
                VideoBack = "Assets/VideoPagePictureYellow2.png",
                PlaylistBack = "Assets/PlaylistPagePictureYellow2.png"
            }
            // Добавьте другие темы
        };
        
        SelectedTheme = Presets.First();
        
        // Подписка на смену темы
        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(theme => ApplyTheme(theme));
    }
    
    private void ApplyTheme(Theme theme)
    {
        // Здесь обновите ресурсы приложения
        var app = Avalonia.Application.Current;
        if (app == null) return;

        // Пример обновления ресурсов
        app.Resources["PrimaryColor"] = Avalonia.Media.Color.Parse(theme.PrimaryColor);
        app.Resources["SecondaryColor"] = Avalonia.Media.Color.Parse(theme.SecondaryColor);
        app.Resources["HomeButton"] = new Avalonia.Media.Imaging.Bitmap(theme.HomeButton);
        app.Resources["MusicButton"] = new Avalonia.Media.Imaging.Bitmap(theme.MusicButton);
        app.Resources["VideoButton"] = new Avalonia.Media.Imaging.Bitmap(theme.VideoButton);
        app.Resources["PlaylistButton"] = new Avalonia.Media.Imaging.Bitmap(theme.PlaylistButton);
        app.Resources["SettingsButton"] = new Avalonia.Media.Imaging.Bitmap(theme.SettingsButton);
        app.Resources["PlayButton"] = new Avalonia.Media.Imaging.Bitmap(theme.PlayButton);
        app.Resources["StopButton"] = new Avalonia.Media.Imaging.Bitmap(theme.StopButton);
        app.Resources["BackTenButton"] = new Avalonia.Media.Imaging.Bitmap(theme.BackTenButton);
        app.Resources["ForeTenButton"] = new Avalonia.Media.Imaging.Bitmap(theme.ForeTenButton);
        app.Resources["BackwardButton"] = new Avalonia.Media.Imaging.Bitmap(theme.BackwardButton);
        app.Resources["ForewardButton"] = new Avalonia.Media.Imaging.Bitmap(theme.ForewardButton);
        app.Resources["EqualizerButton"] = new Avalonia.Media.Imaging.Bitmap(theme.EqualizerButton);
        app.Resources["EditAudioButton"] = new Avalonia.Media.Imaging.Bitmap(theme.EditAudioButton);
        app.Resources["EditVideoButton"] = new Avalonia.Media.Imaging.Bitmap(theme.EditVideoButton);
        app.Resources["VolumeOnButton"] = new Avalonia.Media.Imaging.Bitmap(theme.VolumeOnButton);
        app.Resources["VolumeOffButton"] = new Avalonia.Media.Imaging.Bitmap(theme.VolumeOffButton);
        app.Resources["ListStopBack"] = new Avalonia.Media.Imaging.Bitmap(theme.ListStopBack);
        app.Resources["MainBack"] = new Avalonia.Media.Imaging.Bitmap(theme.MainBack);
        app.Resources["MusicBack"] = new Avalonia.Media.Imaging.Bitmap(theme.MusicBack);
        app.Resources["VideoBack"] = new Avalonia.Media.Imaging.Bitmap(theme.VideoBack);
        app.Resources["PlaylistBack"] = new Avalonia.Media.Imaging.Bitmap(theme.PlaylistBack);
    }
}