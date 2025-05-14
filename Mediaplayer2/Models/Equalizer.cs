using ReactiveUI;

namespace Mediaplayer2.Models;

public class Equalizer : ReactiveObject
{
    private double _bass;
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
    }
}