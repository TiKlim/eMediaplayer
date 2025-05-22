using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Mediaplayer2.Models;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class SettingsPageViewModel : ViewModelBase, IRoutableViewModel
{
    private Equalizer _equalizer;
    
    private string _selectedPreset;
    
    public ObservableCollection<string> PresetNames { get; private set; }
    
    public string Main { get; }
    
    public string PreMain { get; }
    
    public string? UrlPathSegment => "/settings";
    
    public IScreen HostScreen { get; }
    
    public Equalizer Equalizer
    {
        get => _equalizer;
        set => this.RaiseAndSetIfChanged(ref _equalizer, value);
    }
    
    public event Action EqualizerUpdated;
    
    //public Equalizer EqualizerValue { get; }

    public SettingsPageViewModel()
    {
        
    }

    public SettingsPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        
        Main = "Настройки";
        PreMain = "Настрой под настроение";

        //EqualizerValue = new Equalizer();
        
        _equalizer = new Equalizer();
        PresetNames = new ObservableCollection<string>(_equalizer.Presets.Keys);
        
        if (PresetNames.Count > 0)
        {
            SelectedPreset = PresetNames[0];
        }
    }

    public string SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            if (_selectedPreset != value)
            {
                _selectedPreset = value;
                OnPropertyChanged(nameof(SelectedPreset));
                ApplyPreset();
            }
        }
    }
    

    private void ApplyPreset()
    {
        _equalizer.SetPreset(SelectedPreset);
        
        _equalizer.SetPreset(SelectedPreset);
        EqualizerUpdated?.Invoke();
        ApplyEqualizerToAudio();
        ApplyEqualizerToVideo();
    }

    private void ApplyEqualizerToAudio()
    {
        
    }

    private void ApplyEqualizerToVideo()
    {
        
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}