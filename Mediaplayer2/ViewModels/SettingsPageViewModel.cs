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
    private Equalizer _equalizer;
    
    private string _selectedPreset;
    
    private AudioSettings _audioSettings;
    
    //private readonly SettingsPageViewModel _equalizers;
    
    //private EqualizerSampleProvider _equalizerProvider;
    
    public ObservableCollection<string> PresetNames { get; private set; }
    
    public string Main { get; }
    
    public string PreMain { get; }
    
    public string? UrlPathSegment => "/settings";
    
    public IScreen HostScreen { get; }
    
    /*public Equalizer Equalizer
    {
        get => _equalizer;
        set => this.RaiseAndSetIfChanged(ref _equalizer, value);
    }*/
    
    /*public string SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPreset, value);
            _equalizer.SetPreset(value);
            EqualizerUpdated?.Invoke();
        }
    }*/
    
    public string SelectedPreset
    {
        get => _audioSettings.SelectedPreset;
        set => _audioSettings.SelectedPreset = value;
    }
    
    public event Action EqualizerUpdated;
    
    //public Equalizer EqualizerValue { get; }

    public SettingsPageViewModel()
    {
        
    }

    public SettingsPageViewModel(AudioSettings audioSettings, IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        
        Main = "Настройки";
        PreMain = "Настрой под настроение";

        //EqualizerValue = new Equalizer();
        //_equalizer = equalizer;
        _audioSettings = audioSettings;
        
        PresetNames = new ObservableCollection<string>(_audioSettings.Equalizer.Presets.Keys);
        
        SelectedPreset = PresetNames.FirstOrDefault() ?? "";
        
        _equalizer.WhenAnyValue(eq => eq.CurrentSettings)
            .Subscribe(_ => EqualizerUpdated?.Invoke());
    
        if (PresetNames.Count > 0)
        {
            SelectedPreset = PresetNames[0];
        }
    }
    
    /*public void ApplyEqualizer()
    {

        var currentSettings = _equalizer.CurrentSettings;

        for (int i = 0; i < _equalizer.CurrentSettings.Length; i++)
        {
            _equalizerProvider.Gains[i] = _equalizer.CurrentSettings[i];
        }
    }*/

    /*public string SelectedPreset
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
    }*/
    

    /*private void ApplyPreset()
    {
        //_equalizer.SetPreset(SelectedPreset);
        
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
        
    }*/

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}