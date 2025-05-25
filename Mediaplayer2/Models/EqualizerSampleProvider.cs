using System;
using NAudio.Wave;

namespace Mediaplayer2.Models;

public class EqualizerSampleProvider : ISampleProvider
{
    private readonly ISampleProvider source;
    public float[] Gains { get; private set; }

    private readonly int sampleRate;
    private readonly int channels;

    public EqualizerSampleProvider(ISampleProvider source, float[] gains)
    {
        this.source = source ?? throw new ArgumentNullException(nameof(source));
        this.Gains = gains ?? throw new ArgumentNullException(nameof(gains));

        // Проверка на положительное количество каналов
        if (source.WaveFormat.Channels <= 0)
            throw new ArgumentException("Количество каналов должно быть положительным.", nameof(source));

        this.sampleRate = source.WaveFormat.SampleRate;
        this.channels = source.WaveFormat.Channels;

        // Проверка, чтобы количество элементов в Gains соответствовало количеству каналов
        if (Gains.Length < channels)
            throw new ArgumentException($"Массив gains должен содержать как минимум {channels} элементов.", nameof(gains));

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
                float gain = Gains[ch];
                buffer[i] *= gain;
            }
        }

        return samplesRead;
    }
}