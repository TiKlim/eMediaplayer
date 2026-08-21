using System;
using System.Collections.ObjectModel;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Mediaplayer2.Models;

namespace Mediaplayer2.Services;

public interface ILanguageService
{
    // Copyright (c) 2026 Timofeev Klim (eMediaplayer)
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    
    ObservableCollection<Language> Languages { get; }
    Language CurrentLanguage { get; set; }
    
    void ApplyLanguage(Language language);
    void LoadSavedLanguage();
    
    event EventHandler<Language> LanguageChanged;
}