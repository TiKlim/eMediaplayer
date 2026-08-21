using System;
using System.Collections.ObjectModel;
using Mediaplayer2.Models;

namespace Mediaplayer2.Services;

public interface IThemeService
{
    // Copyright (c) 2026 Timofeev Klim (eMediaplayer)
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    
    ObservableCollection<Theme> Themes { get; }
    Theme CurrentTheme { get; set; }
    
    void ApplyTheme(Theme theme);
    void LoadSavedTheme();
    
    event EventHandler<Theme> ThemeChanged;
}