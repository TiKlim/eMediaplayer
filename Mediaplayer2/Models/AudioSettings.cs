using System.Linq;
using ReactiveUI;

namespace Mediaplayer2.Models;

public class AudioSettings : ReactiveObject
{
    private Equalizer _equalizer;
    private string _selectedPreset;

    public Equalizer Equalizer
    {
        get => _equalizer;
        set => this.RaiseAndSetIfChanged(ref _equalizer, value);
    }

    public string SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPreset, value);
            _equalizer.SetPreset(value);
        }
    }

    public AudioSettings()
    {
        _equalizer = new Equalizer();
        SelectedPreset = _equalizer.Presets.Keys.FirstOrDefault() ?? "";
    }
}