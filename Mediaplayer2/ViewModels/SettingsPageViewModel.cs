using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Mediaplayer2.Models;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class SettingsPageViewModel : ReactiveObject, IRoutableViewModel
{
    private const string SettingsFileName = "userSettings.json";

    private string GetSettingsFilePath()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(folder, "Mediaplayer");
        if (!Directory.Exists(appFolder))
            Directory.CreateDirectory(appFolder);
        return Path.Combine(appFolder, SettingsFileName);
    }
    
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
                Name = "Лимонный щербет", 
                PrimaryColor = "#FFCA3A", 
                SecondaryColor = "#F5F3ED", 
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
            },
            new Theme
            {
                Name = "Фисташка", 
                PrimaryColor = "#8AC926", 
                SecondaryColor = "#F0F3EC", 
                HomeButton = "Assets/HomeGreen.png", 
                MusicButton = "Assets/MusicCollectionGreen.png", 
                VideoButton = "Assets/VideoCollectionGreen.png",
                PlaylistButton = "Assets/PlaylistGreen.png",
                SettingsButton = "Assets/SettingsGreen.png",
                PlayButton = "Assets/ButtonPlayGreen.png",
                StopButton = "Assets/StopGreen.png",
                BackTenButton = "Assets/BackTimeGreen.png",
                ForeTenButton = "Assets/ForeTimeGreen.png",
                BackwardButton = "Assets/BackwardGreen.png",
                ForewardButton = "Assets/ForwardGreen.png",
                EqualizerButton = "Assets/EqualizerGreen.png",
                EditAudioButton = "Assets/EditAudioGreen.png",
                EditVideoButton = "Assets/EditVideoGreen.png",
                VolumeOnButton = "Assets/VolumeOnGreen.png",
                VolumeOffButton = "Assets/VolumeOffGreen.png",
                ListStopBack = "Assets/ListStopGreen.png",
                MainBack = "Assets/MainPagePictureGreen2.png",
                MusicBack = "Assets/MusicPagePictureGreen2.png",
                VideoBack = "Assets/VideoPagePictureGreen2.png",
                PlaylistBack = "Assets/PlaylistPagePictureGreen2.png"
            },
            new Theme
            {
                Name = "Голубика", 
                PrimaryColor = "#1982C4", 
                SecondaryColor = "#ECF0F3", 
                HomeButton = "Assets/HomeBlue.png", 
                MusicButton = "Assets/MusicCollectionBlue.png", 
                VideoButton = "Assets/VideoCollectionBlue.png",
                PlaylistButton = "Assets/PlaylistBlue.png",
                SettingsButton = "Assets/SettingsBlue.png",
                PlayButton = "Assets/ButtonPlayBlue.png",
                StopButton = "Assets/StopBlue.png",
                BackTenButton = "Assets/BackTimeBlue.png",
                ForeTenButton = "Assets/ForeTimeBlue.png",
                BackwardButton = "Assets/BackwardBlue.png",
                ForewardButton = "Assets/ForwardBlue.png",
                EqualizerButton = "Assets/EqualizerBlue.png",
                EditAudioButton = "Assets/EditAudioBlue.png",
                EditVideoButton = "Assets/EditVideoBlue.png",
                VolumeOnButton = "Assets/VolumeOnBlue.png",
                VolumeOffButton = "Assets/VolumeOffBlue.png",
                ListStopBack = "Assets/ListStopBlue.png",
                MainBack = "Assets/MainPagePictureBlue2.png",
                MusicBack = "Assets/MusicPagePictureBlue2.png",
                VideoBack = "Assets/VideoPagePictureBlue2.png",
                PlaylistBack = "Assets/PlaylistPagePictureBlue2.png"
            },
            new Theme
            {
                Name = "Черничный пломбир", 
                PrimaryColor = "#6A4C93", 
                SecondaryColor = "#EFEEF1", 
                HomeButton = "Assets/HomeViolet.png", 
                MusicButton = "Assets/MusicCollectionViolet.png", 
                VideoButton = "Assets/VideoCollectionViolet.png",
                PlaylistButton = "Assets/PlaylistViolet.png",
                SettingsButton = "Assets/SettingsViolet.png",
                PlayButton = "Assets/ButtonPlayViolet.png",
                StopButton = "Assets/StopViolet.png",
                BackTenButton = "Assets/BackTimeViolet.png",
                ForeTenButton = "Assets/ForeTimeViolet.png",
                BackwardButton = "Assets/BackwardViolet.png",
                ForewardButton = "Assets/ForwardViolet.png",
                EqualizerButton = "Assets/EqualizerViolet.png",
                EditAudioButton = "Assets/EditAudioViolet.png",
                EditVideoButton = "Assets/EditVideoViolet.png",
                VolumeOnButton = "Assets/VolumeOnViolet.png",
                VolumeOffButton = "Assets/VolumeOffViolet.png",
                ListStopBack = "Assets/ListStopViolet.png",
                MainBack = "Assets/MainPagePictureViolet2.png",
                MusicBack = "Assets/MusicPagePictureViolet2.png",
                VideoBack = "Assets/VideoPagePictureViolet2.png",
                PlaylistBack = "Assets/PlaylistPagePictureViolet2.png"
            }
        };
        
        SelectedTheme = Presets.First();
        
        // Подписка на смену темы
        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(theme => ApplyTheme(theme));
        
        var savedThemeName = LoadSelectedThemeName();
        if (!string.IsNullOrEmpty(savedThemeName))
        {
            var savedTheme = Presets.FirstOrDefault(t => t.Name == savedThemeName);
            if (savedTheme != null)
                SelectedTheme = savedTheme;
            else
                SelectedTheme = Presets.First();
        }
        else
        {
            SelectedTheme = Presets.First();
        }
        
        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(theme =>
            {
                ApplyTheme(theme);
                SaveSelectedThemeName(theme?.Name);
            });
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
                Name = "Лимонный щербет", 
                PrimaryColor = "#FFCA3A", 
                SecondaryColor = "#F5F3ED", 
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
            },
            new Theme
            {
                Name = "Фисташка", 
                PrimaryColor = "#8AC926", 
                SecondaryColor = "#F0F3EC", 
                HomeButton = "Assets/HomeGreen.png", 
                MusicButton = "Assets/MusicCollectionGreen.png", 
                VideoButton = "Assets/VideoCollectionGreen.png",
                PlaylistButton = "Assets/PlaylistGreen.png",
                SettingsButton = "Assets/SettingsGreen.png",
                PlayButton = "Assets/ButtonPlayGreen.png",
                StopButton = "Assets/StopGreen.png",
                BackTenButton = "Assets/BackTimeGreen.png",
                ForeTenButton = "Assets/ForeTimeGreen.png",
                BackwardButton = "Assets/BackwardGreen.png",
                ForewardButton = "Assets/ForwardGreen.png",
                EqualizerButton = "Assets/EqualizerGreen.png",
                EditAudioButton = "Assets/EditAudioGreen.png",
                EditVideoButton = "Assets/EditVideoGreen.png",
                VolumeOnButton = "Assets/VolumeOnGreen.png",
                VolumeOffButton = "Assets/VolumeOffGreen.png",
                ListStopBack = "Assets/ListStopGreen.png",
                MainBack = "Assets/MainPagePictureGreen2.png",
                MusicBack = "Assets/MusicPagePictureGreen2.png",
                VideoBack = "Assets/VideoPagePictureGreen2.png",
                PlaylistBack = "Assets/PlaylistPagePictureGreen2.png"
            },
            new Theme
            {
                Name = "Голубика", 
                PrimaryColor = "#1982C4", 
                SecondaryColor = "#ECF0F3", 
                HomeButton = "Assets/HomeBlue.png", 
                MusicButton = "Assets/MusicCollectionBlue.png", 
                VideoButton = "Assets/VideoCollectionBlue.png",
                PlaylistButton = "Assets/PlaylistBlue.png",
                SettingsButton = "Assets/SettingsBlue.png",
                PlayButton = "Assets/ButtonPlayBlue.png",
                StopButton = "Assets/StopBlue.png",
                BackTenButton = "Assets/BackTimeBlue.png",
                ForeTenButton = "Assets/ForeTimeBlue.png",
                BackwardButton = "Assets/BackwardBlue.png",
                ForewardButton = "Assets/ForwardBlue.png",
                EqualizerButton = "Assets/EqualizerBlue.png",
                EditAudioButton = "Assets/EditAudioBlue.png",
                EditVideoButton = "Assets/EditVideoBlue.png",
                VolumeOnButton = "Assets/VolumeOnBlue.png",
                VolumeOffButton = "Assets/VolumeOffBlue.png",
                ListStopBack = "Assets/ListStopBlue.png",
                MainBack = "Assets/MainPagePictureBlue2.png",
                MusicBack = "Assets/MusicPagePictureBlue2.png",
                VideoBack = "Assets/VideoPagePictureBlue2.png",
                PlaylistBack = "Assets/PlaylistPagePictureBlue2.png"
            },
            new Theme
            {
                Name = "Черничный пломбир", 
                PrimaryColor = "#6A4C93", 
                SecondaryColor = "#EFEEF1", 
                HomeButton = "Assets/HomeViolet.png", 
                MusicButton = "Assets/MusicCollectionViolet.png", 
                VideoButton = "Assets/VideoCollectionViolet.png",
                PlaylistButton = "Assets/PlaylistViolet.png",
                SettingsButton = "Assets/SettingsViolet.png",
                PlayButton = "Assets/ButtonPlayViolet.png",
                StopButton = "Assets/StopViolet.png",
                BackTenButton = "Assets/BackTimeViolet.png",
                ForeTenButton = "Assets/ForeTimeViolet.png",
                BackwardButton = "Assets/BackwardViolet.png",
                ForewardButton = "Assets/ForwardViolet.png",
                EqualizerButton = "Assets/EqualizerViolet.png",
                EditAudioButton = "Assets/EditAudioViolet.png",
                EditVideoButton = "Assets/EditVideoViolet.png",
                VolumeOnButton = "Assets/VolumeOnViolet.png",
                VolumeOffButton = "Assets/VolumeOffViolet.png",
                ListStopBack = "Assets/ListStopViolet.png",
                MainBack = "Assets/MainPagePictureViolet2.png",
                MusicBack = "Assets/MusicPagePictureViolet2.png",
                VideoBack = "Assets/VideoPagePictureViolet2.png",
                PlaylistBack = "Assets/PlaylistPagePictureViolet2.png"
            }
        };
        
        SelectedTheme = Presets.First();
        
        // Подписка на смену темы
        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(theme => ApplyTheme(theme));
        
        var savedThemeName = LoadSelectedThemeName();
        if (!string.IsNullOrEmpty(savedThemeName))
        {
            var savedTheme = Presets.FirstOrDefault(t => t.Name == savedThemeName);
            if (savedTheme != null)
                SelectedTheme = savedTheme;
            else
                SelectedTheme = Presets.First();
        }
        else
        {
            SelectedTheme = Presets.First();
        }
        
        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(theme =>
            {
                ApplyTheme(theme);
                SaveSelectedThemeName(theme?.Name);
            });

    }
    
    public void ApplyTheme(Theme theme)
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
    
    private void SaveSelectedThemeName(string themeName)
    {
        try
        {
            var settings = new UserSettings { SelectedThemeName = themeName };
            var json = JsonSerializer.Serialize(settings);
            var path = GetSettingsFilePath();
            File.WriteAllText(path, json);
        }
        catch
        {
            // Логируйте ошибки при необходимости
        }
    }
    
    public string? LoadSelectedThemeName()
    {
        try
        {
            var path = GetSettingsFilePath();
            if (!File.Exists(path))
                return null;
            var json = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<UserSettings>(json);
            return settings?.SelectedThemeName;
        }
        catch
        {
            return null;
        }
    }
}