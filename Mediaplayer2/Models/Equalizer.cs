using System.Collections.Generic;
using ReactiveUI;

namespace Mediaplayer2.Models;

public class Equalizer : ReactiveObject
{
    /*private double _bass;
    private double _mid;
    private double _treble;

    public double Bass
    {
        get => _bass;
        set => this.RaiseAndSetIfChanged(ref _bass, value);
    }

    public double Mid
    {
        get => _mid;
        set => this.RaiseAndSetIfChanged(ref _mid, value);
    }

    public double Treble
    {
        get => _treble;
        set => this.RaiseAndSetIfChanged(ref _treble, value);
    }*/
    
    public Dictionary<string, float[]> Presets { get; private set; }
    public float[] CurrentSettings { get; private set; }

    public Equalizer()
    {
        Presets = new Dictionary<string, float[]>
        {
            { "Pop", new float[] { 0.5f, 1.0f, 0.8f, 0.5f, 0.3f } },
            { "Rock", new float[] { 0.7f, 0.9f, 1.0f, 0.6f, 0.4f } },
            { "Jazz", new float[] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f } },
            { "Classical", new float[] { 0.6f, 0.7f, 0.8f, 0.9f, 0.7f } }
        };
        //CurrentSettings = new float[5];
        CurrentSettings = Presets["Pop"];
    }

    public void SetPreset(string presetName)
    {
        if (Presets.ContainsKey(presetName))
        {
            CurrentSettings = Presets[presetName];
        }
    }
}