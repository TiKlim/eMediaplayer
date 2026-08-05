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
                Name = "Голубика", 
                PrimaryColor = "#1982c4", 
                SecondaryColor = "#cce4f2", 
                ThirdColor = "#e6f1f9",
                FourthColor = "#8cc0e1",
                PlayButton = "Assets/FocusWhite.png",
                StopButton = "Assets/StopWhite.png",
                BackTenButton = "Assets/BackTimeWhite.png",
                ForeTenButton = "Assets/ForeTimeWhite.png",
                VolumeOnButton = "Assets/VolumeOnWhite.png",
                VolumeOffButton = "Assets/VolumeOffWhite.png",
                MainBack = "Assets/MainPagePictureBlue2.png",
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
                PlayButton = "Assets/FocusWhite.png",
                StopButton = "Assets/StopWhite.png",
                BackTenButton = "Assets/BackTimeWhite.png",
                ForeTenButton = "Assets/ForeTimeWhite.png",
                VolumeOnButton = "Assets/VolumeOnWhite.png",
                VolumeOffButton = "Assets/VolumeOffWhite.png",
                MainBack = "Assets/MainPagePictureViolet2.png",
                Cover = "Assets/grape.jpg",
                Close = "Assets/CloseViolet.png",
                Roll = "Assets/RollViolet.png",
                Maximise = "Assets/MaximiseViolet.png",
                Minimize = "Assets/MinimizeViolet.png"
            },
            new Theme
            {
                Name = "Клубника", 
                PrimaryColor = "#ff595e", 
                SecondaryColor = "#ffdadc", 
                ThirdColor = "#ffeded",
                FourthColor = "#ffacae",
                PlayButton = "Assets/FocusWhite.png",
                StopButton = "Assets/StopWhite.png",
                BackTenButton = "Assets/BackTimeWhite.png",
                ForeTenButton = "Assets/ForeTimeWhite.png",
                VolumeOnButton = "Assets/VolumeOnWhite.png",
                VolumeOffButton = "Assets/VolumeOffWhite.png",
                MainBack = "Assets/MainPagePictureRed2.png",
                Cover = "Assets/strawberry.jpg",
                Close = "Assets/CloseRed.png",
                Roll = "Assets/RollRed.png",
                Maximise = "Assets/MaximiseRed.png",
                Minimize = "Assets/MinimizeRed.png"
            },
            new Theme
            {
                Name = "Манго", 
                PrimaryColor = "#ffae43", 
                SecondaryColor = "#ffedd6",
                ThirdColor = "#fff6ea",
                FourthColor = "#ffd6a1",
                PlayButton = "Assets/FocusWhite.png",
                StopButton = "Assets/StopWhite.png",
                BackTenButton = "Assets/BackTimeWhite.png",
                ForeTenButton = "Assets/ForeTimeWhite.png",
                VolumeOnButton = "Assets/VolumeOnWhite.png",
                VolumeOffButton = "Assets/VolumeOffWhite.png",
                MainBack = "Assets/MainPagePictureOrange.png",
                Cover = "Assets/lemon sherbet.jpg",
                Close = "Assets/CloseOrange.png",
                Roll = "Assets/RollOrange.png",
                Maximise = "Assets/MaximiseOrange.png",
                Minimize = "Assets/MinimizeOrange.png"
            },
            new Theme
            {
                Name = "Фисташка", 
                PrimaryColor = "#8ac926", 
                SecondaryColor = "#e5f3cf", 
                ThirdColor = "#f2f9e7",
                FourthColor = "#c4e492",
                PlayButton = "Assets/FocusWhite.png",
                StopButton = "Assets/StopWhite.png",
                BackTenButton = "Assets/BackTimeWhite.png",
                ForeTenButton = "Assets/ForeTimeWhite.png",
                VolumeOnButton = "Assets/VolumeOnWhite.png",
                VolumeOffButton = "Assets/VolumeOffWhite.png",
                MainBack = "Assets/MainPagePictureGreen2.png",
                Cover = "Assets/pistachios.jpg",
                Close = "Assets/CloseGreen.png",
                Roll = "Assets/RollGreen.png",
                Maximise = "Assets/MaximiseGreen.png",
                Minimize = "Assets/MinimizeGreen.png"
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
        app.Resources["PlayButton"] = new Avalonia.Media.Imaging.Bitmap(theme.PlayButton);
        app.Resources["StopButton"] = new Avalonia.Media.Imaging.Bitmap(theme.StopButton);
        app.Resources["BackTenButton"] = new Avalonia.Media.Imaging.Bitmap(theme.BackTenButton);
        app.Resources["ForeTenButton"] = new Avalonia.Media.Imaging.Bitmap(theme.ForeTenButton);
        app.Resources["VolumeOnButton"] = new Avalonia.Media.Imaging.Bitmap(theme.VolumeOnButton);
        app.Resources["VolumeOffButton"] = new Avalonia.Media.Imaging.Bitmap(theme.VolumeOffButton);
        app.Resources["MainBack"] = new Avalonia.Media.Imaging.Bitmap(theme.MainBack);
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