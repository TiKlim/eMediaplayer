using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia.Controls;
using NAudio.Wave;
using ReactiveUI;

namespace Mediaplayer2.ViewModels;

public class MusicPageViewModel : ViewModelBase
{
    public string Main { get; } = "Аудиоплеер";
    
    public string PreMain { get; } = "Что послушаем сегодня?";
    
    private string _filePath;
    private IWavePlayer _waveOut;
    private AudioFileReader _audioFileReader;
    private TimeSpan _totalTime;
    private TimeSpan _audioDuration;
    private bool _isPlaying = false;
    
    public ICommand LoadFileCommand { get; }

    public MusicPageViewModel()
    {
        LoadFileCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "MP3 Files", Extensions = { "mp3" } }
                }
            };
            var result = await dialog.ShowAsync(this);
            if (result.Length > 0)
            {
                _filePath = result[0];
                LoadMp3Info(_filePath); // ������� ��������� ���������� � �����
                _audioFileReader = new AudioFileReader(_filePath);
                _audioDuration = _audioFileReader.TotalTime; // �������� ������������ ����������
                PlaybackSlider.Maximum = _audioDuration.TotalSeconds; // ������������� ������������ �������� ��� �������� ���������������
                //StartSlider.Maximum = _audioDuration.TotalSeconds; // ������������� ������������ �������� ��� �������� ������
                //EndSlider.Maximum = _audioDuration.TotalSeconds; // ������������� ������������ �������� ��� �������� �����

                // ��������� ����������� ������������
                //EndSlider.Value = _audioDuration.TotalSeconds; // ������������� ����� �� ������������ ������������
                //StartSlider.Value = 0; // �������� � ����
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_audioFileReader); // ������ �������������� �������������
            }
        });
    }
}