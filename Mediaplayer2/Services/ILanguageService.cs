using System;
using System.Collections.ObjectModel;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Mediaplayer2.Models;

namespace Mediaplayer2.Services;

public interface ILanguageService
{
    ObservableCollection<Language> Languages { get; }
    Language CurrentLanguage { get; set; }
    
    void ApplyLanguage(Language language);
    void LoadSavedLanguage();
    
    event EventHandler<Language> LanguageChanged;
}