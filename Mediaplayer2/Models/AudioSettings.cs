using System.Linq;
using ReactiveUI;

namespace Mediaplayer2.Models;

public class AudioSettings : ReactiveObject
{
    // Copyright (c) 2026 Timofeev Klim (eMediaplayer)
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    
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