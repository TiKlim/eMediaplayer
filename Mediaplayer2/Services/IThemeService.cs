using System;
using System.Collections.ObjectModel;
using Mediaplayer2.Models;

namespace Mediaplayer2.Services;

public interface IThemeService
{
    ObservableCollection<Theme> Themes { get; }
    Theme CurrentTheme { get; set; }
    
    void ApplyTheme(Theme theme);
    void LoadSavedTheme();
    
    event EventHandler<Theme> ThemeChanged;
}