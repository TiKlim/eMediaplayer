using System.Collections.Generic;
using ReactiveUI;

namespace Mediaplayer2.Models;

public class Equalizer : ReactiveObject
{
    private float[] _currentSettings;

    public Dictionary<string, float[]> Presets { get; }

    public float[] CurrentSettings
    {
        get => _currentSettings;
        private set => this.RaiseAndSetIfChanged(ref _currentSettings, value);
    }

    public Equalizer()
    {
        Presets = new Dictionary<string, float[]>
        {
            { "Pop", new float[] { 0.5f, 1.0f, 0.8f, 0.5f, 0.3f } },
            { "Rock", new float[] { 0.7f, 0.9f, 1.0f, 0.6f, 0.4f } },
            { "Jazz", new float[] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f } },
            { "Classical", new float[] { 0.6f, 0.7f, 0.8f, 0.9f, 0.7f } }
        };
        CurrentSettings = Presets["Pop"];
    }

    public void SetPreset(string presetName)
    {
        if (Presets.TryGetValue(presetName, out var settings))
        {
            CurrentSettings = settings;
        }
    }
}