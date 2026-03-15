using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Mediaplayer2.Models;

namespace Mediaplayer2.Services;

public class ThemeService : IThemeService
{
    private const string SettingsFileName = "userSettings.json";
    private Theme _currentTheme;
    
    public ObservableCollection<Theme> Themes { get; }

    public Theme CurrentTheme
    {
        get => _currentTheme;
        set
        {
            if (_currentTheme != value)
            {
                _currentTheme = value;
                ApplyTheme(value);
                ThemeChanged?.Invoke(this, value);
                SaveThemeToSettings(value.Name);
            }
        }
    }
    
    public event EventHandler<Theme> ThemeChanged;

    public ThemeService()
    {
        Themes = new ObservableCollection<Theme>
        {
            new Theme
            {
                Name = "Клубника", 
                PrimaryColor = "#ff595e", 
                SecondaryColor = "#ffdadc", 
                ThirdColor = "#ffeded",
                FourthColor = "#ffacae",
                HomeButton = "Assets/HomeRed.png", 
                MusicButton = "Assets/MusicCollectionRed.png", 
                VideoButton = "Assets/VideoCollectionRed.png",
                PlaylistButton = "Assets/PlaylistRed.png",
                SettingsButton = "Assets/SettingsRed.png",
                PlayButton = "Assets/FocusRed.png",
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
                PlaylistBack = "Assets/PlaylistPagePictureRed2.png",
                Cover = "Assets/strawberry.jpg",
                Close = "Assets/CloseRed.png",
                Roll = "Assets/RollRed.png",
                Maximise = "Assets/MaximiseRed.png",
                Minimize = "Assets/MinimizeRed.png"
            },
            new Theme
            {
                Name = "Банан", 
                PrimaryColor = "#ffca3a", 
                SecondaryColor = "#fff3d4",
                ThirdColor = "#fff9e9",
                FourthColor = "#ffe49c",
                HomeButton = "Assets/HomeYello.png", 
                MusicButton = "Assets/MusicCollectionYello.png", 
                VideoButton = "Assets/VideoCollectionYello.png",
                PlaylistButton = "Assets/PlaylistYello.png",
                SettingsButton = "Assets/SettingsYello.png",
                PlayButton = "Assets/FocusYellow.png",
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
                PlaylistBack = "Assets/PlaylistPagePictureYellow2.png",
                Cover = "Assets/lemon sherbet.jpg",
                Close = "Assets/CloseYellow.png",
                Roll = "Assets/RollYellow.png",
                Maximise = "Assets/MaximiseYellow.png",
                Minimize = "Assets/MinimizeYellow.png"
            },
            new Theme
            {
                Name = "Фисташка", 
                PrimaryColor = "#8ac926", 
                SecondaryColor = "#e5f3cf", 
                ThirdColor = "#f2f9e7",
                FourthColor = "#c4e492",
                HomeButton = "Assets/HomeGreen.png", 
                MusicButton = "Assets/MusicCollectionGreen.png", 
                VideoButton = "Assets/VideoCollectionGreen.png",
                PlaylistButton = "Assets/PlaylistGreen.png",
                SettingsButton = "Assets/SettingsGreen.png",
                PlayButton = "Assets/FocusGreen.png",
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
                PlaylistBack = "Assets/PlaylistPagePictureGreen2.png",
                Cover = "Assets/pistachios.jpg",
                Close = "Assets/CloseGreen.png",
                Roll = "Assets/RollGreen.png",
                Maximise = "Assets/MaximiseGreen.png",
                Minimize = "Assets/MinimizeGreen.png"
            },
            new Theme
            {
                Name = "Голубика", 
                PrimaryColor = "#1982c4", 
                SecondaryColor = "#cce4f2", 
                ThirdColor = "#e6f1f9",
                FourthColor = "#8cc0e1", 
                HomeButton = "Assets/HomeBlue.png", 
                MusicButton = "Assets/MusicCollectionBlue.png", 
                VideoButton = "Assets/VideoCollectionBlue.png",
                PlaylistButton = "Assets/PlaylistBlue.png",
                SettingsButton = "Assets/SettingsBlue.png",
                PlayButton = "Assets/FocusBlue.png",
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
                PlaylistBack = "Assets/PlaylistPagePictureBlue2.png",
                Cover = "Assets/blueberry.jpg",
                Close = "Assets/CloseBlue.png",
                Roll = "Assets/RollBlue.png",
                Maximise = "Assets/MaximiseBlue.png",
                Minimize = "Assets/MinimizeBlue.png"
            },
            new Theme
            {
                Name = "Ежевика", 
                PrimaryColor = "#6a4c93", 
                SecondaryColor = "#ded8e7", 
                ThirdColor = "#efebf3",
                FourthColor = "#b4a5c9", 
                HomeButton = "Assets/HomeViolet.png", 
                MusicButton = "Assets/MusicCollectionViolet.png", 
                VideoButton = "Assets/VideoCollectionViolet.png",
                PlaylistButton = "Assets/PlaylistViolet.png",
                SettingsButton = "Assets/SettingsViolet.png",
                PlayButton = "Assets/FocusViolet.png",
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
                PlaylistBack = "Assets/PlaylistPagePictureViolet2.png",
                Cover = "Assets/grape.jpg",
                Close = "Assets/CloseViolet.png",
                Roll = "Assets/RollViolet.png",
                Maximise = "Assets/MaximiseViolet.png",
                Minimize = "Assets/MinimizeViolet.png"
            }
        };
        
        LoadSavedTheme();
    }
    
    // Обновление ресурсов приложения
    public void ApplyTheme(Theme theme)
    {
        var app = Application.Current;
        if (app == null) return;
        
        // Оттенки цвета
        app.Resources["PrimaryColor"] = Avalonia.Media.Color.Parse(theme.PrimaryColor);
        app.Resources["SecondaryColor"] = Avalonia.Media.Color.Parse(theme.SecondaryColor);
        app.Resources["ThirdColor"] = Avalonia.Media.Color.Parse(theme.ThirdColor);
        app.Resources["FourthColor"] = Avalonia.Media.Color.Parse(theme.FourthColor);
        // Кнопки
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
        app.Resources["Cover"] = new Avalonia.Media.Imaging.Bitmap(theme.Cover);
        app.Resources["Close"] = new Avalonia.Media.Imaging.Bitmap(theme.Close);
        app.Resources["Roll"] = new Avalonia.Media.Imaging.Bitmap(theme.Roll);
        app.Resources["Minimize"] = new Avalonia.Media.Imaging.Bitmap(theme.Minimize);
        app.Resources["Maximise"] = new Avalonia.Media.Imaging.Bitmap(theme.Maximise);
    }
    
    // Загрузка сохранённой темы
    public void LoadSavedTheme()
    {
        var savedThemeName = LoadSelectedThemeName();
        if (!string.IsNullOrEmpty(savedThemeName))
        {
            var savedTheme = Themes.FirstOrDefault(t => t.Name == savedThemeName);
            if (savedTheme != null)
            {
                _currentTheme = savedTheme;
                ApplyTheme(savedTheme);
            }
            else
            {
                _currentTheme = Themes.First();
            }
        }
        else
        {
            _currentTheme = Themes.First();
        }
    }
    
    private string GetSettingsFilePath()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(folder, "Mediaplayer");
        if (!Directory.Exists(appFolder))
            Directory.CreateDirectory(appFolder);
        return Path.Combine(appFolder, "userSettings.json");
    }
    
    private string? LoadSelectedThemeName()
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
    
    private void SaveThemeToSettings(string themeName)
    {
        try
        {
            var path = GetSettingsFilePath();
            UserSettings settings;
            
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                settings = JsonSerializer.Deserialize<UserSettings>(json) ?? new UserSettings();
            }
            else
            {
                settings = new UserSettings();
            }
            
            settings.SelectedThemeName = themeName;
            
            var newJson = JsonSerializer.Serialize(settings);
            File.WriteAllText(path, newJson);
        }
        catch
        {
            // Логи
        }
    }
}