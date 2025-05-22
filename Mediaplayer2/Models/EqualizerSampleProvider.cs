using NAudio.Wave;

namespace Mediaplayer2.Models;

public class EqualizerSampleProvider : ISampleProvider
{
    private readonly ISampleProvider source;
    private readonly float[] gains;

    private readonly int sampleRate;
    private readonly int channels;


    public EqualizerSampleProvider(ISampleProvider source, float[] gains)
    {
        this.source = source;
        this.gains = gains;
        this.sampleRate = source.WaveFormat.SampleRate;
        this.channels = source.WaveFormat.Channels;
        WaveFormat = source.WaveFormat;
    }

    public WaveFormat WaveFormat { get; }

    public int Read(float[] buffer, int offset, int count)
    {
        int samplesRead = source.Read(buffer, offset, count);
        
        for (int n = 0; n < samplesRead; n += channels)
        {
            for (int ch = 0; ch < channels; ch++)
            {
                int i = offset + n + ch;

                float gain = 1f;
                if (gains.Length > 0)
                    gain = gains[0]; 

                buffer[i] *= gain;
            }
        }

        return samplesRead;
    }
}