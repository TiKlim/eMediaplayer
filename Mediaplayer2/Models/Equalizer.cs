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
            { "Pop", new float[10] { 0.5f, 1.0f, 0.8f, 0.5f, 0.3f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f } },
            { "Vocal", new float[10] { 0f, 1f, 2f, 3f, 2f, 1f, 0f, 0f, 0f, 0f } },
            { "Rock", new float[10] { 0.7f, 0.9f, 1.0f, 0.6f, 0.4f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f } },
            { "Jazz", new float[10] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f } },
            { "Classical", new float[10] { 0.6f, 0.7f, 0.8f, 0.9f, 0.7f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f } },
            { "Bass Boost", new float[10] { 5f, 4f, 3f, 2f, 1f, 0f, 0f, 0f, 0f, 0f } }
        };
        CurrentSettings = (float[])Presets["Pop"].Clone(); // Клонируем массив, чтобы избежать изменения оригинала
    }

    public void SetPreset(string presetName)
    {
        if (Presets.TryGetValue(presetName, out var settings))
        {
            CurrentSettings = (float[])settings.Clone(); // Клонируем, чтобы избежать изменения оригинала
        }
    }
}