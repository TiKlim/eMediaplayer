using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia;
using Mediaplayer2.Models;

namespace Mediaplayer2.Services;

public class LanguageService : ILanguageService
{
    private const string SettingsFileName = "userSettings.json";
    private Language _currentLanguage;
    
    public ObservableCollection<Language> Languages { get; }
    
    public Language CurrentLanguage
    {
        get => _currentLanguage;
        set
        {
            if (_currentLanguage != value)
            {
                _currentLanguage = value;
                ApplyLanguage(value);
                LanguageChanged?.Invoke(this, value);
                SaveLanguageToSettings(value.LanguageName);
            }
        }
    }
    
    public event EventHandler<Language> LanguageChanged;
    
    public LanguageService()
    {
        // Инициализация языков
        Languages = new ObservableCollection<Language>
        {
            new Language
            {
                LanguageName = "Русский",
                RedThemeName = "Клубника",
                OrangeThemeName = "Манго",
                GreenThemeName = "Фисташка",
                BlueThemeName = "Голубика",
                VioletThemeName = "Ежевика",
                FindFileButton = "Найти файл",
                EditFileButton = "Редактировать",
                FirstEqualizerThemeName = "Поп",
                SecondEqualizerThemeName = "Вокал",
                ThirdEqualizerThemeName = "Рок",
                FourthEqualizerThemeName = "Джаз",
                FifthEqualizerThemeName = "Классический",
                SixthEqualizerThemeName = "Усиление низких частот"
            },
            new Language
            {
                LanguageName = "English",
                RedThemeName = "Strawberry",
                OrangeThemeName = "Mango",
                GreenThemeName = "Pistachio",
                BlueThemeName = "Blueberry",
                VioletThemeName = "Blackberry",
                FindFileButton = "Find the file",
                EditFileButton = "Edit",
                FirstEqualizerThemeName = "Pop",
                SecondEqualizerThemeName = "Vocal",
                ThirdEqualizerThemeName = "Rock",
                FourthEqualizerThemeName = "Jazz",
                FifthEqualizerThemeName = "Classical",
                SixthEqualizerThemeName = "Bass"
            }
        };
        LoadSavedLanguage();
    }
    
    public void ApplyLanguage(Language language)
    {
        var app = Application.Current;
        if (app == null) return;

        app.Resources["LanguageName"] = language.LanguageName;
        app.Resources["RedThemeName"] = language.RedThemeName;
        app.Resources["OrangeThemeName"] = language.OrangeThemeName;
        app.Resources["GreenThemeName"] = language.GreenThemeName;
        app.Resources["BlueThemeName"] = language.BlueThemeName;
        app.Resources["VioletThemeName"] = language.VioletThemeName;
        app.Resources["FindFileButton"] = language.FindFileButton;
        app.Resources["EditFileButton"] = language.EditFileButton;
        app.Resources["FirstEqualizerThemeName"] = language.FirstEqualizerThemeName;
        app.Resources["SecondEqualizerThemeName"] = language.SecondEqualizerThemeName;
        app.Resources["ThirdEqualizerThemeName"] = language.ThirdEqualizerThemeName;
        app.Resources["FourthEqualizerThemeName"] = language.FourthEqualizerThemeName;
        app.Resources["FifthEqualizerThemeName"] = language.FifthEqualizerThemeName;
        app.Resources["SixthEqualizerThemeName"] = language.SixthEqualizerThemeName;
    }
    
    // Загрузка сохраненного языка
    public void LoadSavedLanguage()
    {
        var savedLanguageName = LoadSelectedLanguage();
        if (!string.IsNullOrEmpty(savedLanguageName))
        {
            var savedLang = Languages.FirstOrDefault(l => l.LanguageName == savedLanguageName);
            if (savedLang != null)
            {
                _currentLanguage = savedLang;
                ApplyLanguage(savedLang);
            }
            else
            {
                _currentLanguage = Languages.First();
            }
        }
        else
        {
            _currentLanguage = Languages.First();
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
    
    private string? LoadSelectedLanguage()
    {
        try
        {
            var path = GetSettingsFilePath();
            if (!File.Exists(path))
                return null;
            var json = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<UserSettings>(json);
            return settings?.SelectedLanguageName;
        }
        catch
        {
            return null;
        }
    }
    
    private void SaveLanguageToSettings(string languageName)
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
            
            settings.SelectedLanguageName = languageName;
            
            var newJson = JsonSerializer.Serialize(settings);
            File.WriteAllText(path, newJson);
        }
        catch
        {
            // Логи
        }
    }
}