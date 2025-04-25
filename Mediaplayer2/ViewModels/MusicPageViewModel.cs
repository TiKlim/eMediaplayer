using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Mediaplayer2.Views;
using NAudio.Wave;
using ReactiveUI;

namespace Mediaplayer2.ViewModels;

public class MusicPageViewModel : ReactiveObject
{
    private string _main;

    private string _premain;

    private Bitmap? trackImage;
    
    private double _opacityImage;

    public string Main
    {
        get => _main;
        set => this.RaiseAndSetIfChanged(ref _main, value);
    }

    public string PreMain
    {
        get => _premain;
        set => this.RaiseAndSetIfChanged(ref _premain, value);
    }

    public Bitmap? TrackImage
    {
        get => trackImage;
        set => this.RaiseAndSetIfChanged(ref trackImage, value);
    }

    public double OpacityImage
    {
        get => _opacityImage;
        set => this.RaiseAndSetIfChanged(ref _opacityImage, value);
    }

    public double Value { get; set; }
    
    private string _filePath;
    private IWavePlayer _waveOut;
    private AudioFileReader _audioFileReader;
    private TimeSpan _totalTime;
    private TimeSpan _audioDuration;
    private bool _isPlaying = false;
    
    private double _volume;

    public double Volume 
    {
        get => _volume;
        set => this.RaiseAndSetIfChanged(ref _volume, value);
    }
    
    public ICommand LoadFileCommand { get; }
    
    public RoutingState Router { get; } = new RoutingState();
    
    public ReactiveCommand<Unit, IRoutableViewModel> ToHome { get; }
    
    public ICommand PlayPauseCommand { get; }

    public MusicPageViewModel()
    {
        Main = "Аудиоплеер";
        PreMain = "Что послушаем сегодня?";
        TrackImage = new Bitmap("Assets/MusicPagePictureRed.png");
        OpacityImage = 0.2;
        
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
            var desctop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            var result = await dialog.ShowAsync(desctop.MainWindow);
            if (result.Length > 0)
            {
                _filePath = result[0];
                LoadMp3Info(_filePath); 
                _audioFileReader = new AudioFileReader(_filePath);
                _audioDuration = _audioFileReader.TotalTime; 
                Value = _audioDuration.TotalSeconds;
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_audioFileReader); 
            }
        });

        PlayPauseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            _isPlaying = true;
            await PlayAudioAsync(_filePath);
            //_audioFileReader.CurrentTime = TimeSpan.FromSeconds(StartSlider.Value);
            _isPlaying = true;
            await PlayAudioAsync(_filePath);
        });
    }
    
    private void LoadMp3Info(string filePath)
    {
        var file = TagLib.File.Create(filePath);
        
        string title = file.Tag.Title ?? "Нет названия";
        
        string performer = file.Tag.Performers.Length > 0 ? file.Tag.Performers[0] : "Нет исполнителя";
        
        Main = title;
        PreMain = performer;
        
        if (file.Tag.Pictures.Length > 0)
        {
            var picture = file.Tag.Pictures[0];
            using (var stream = new MemoryStream(picture.Data.Data))
            {
                TrackImage = new Bitmap(stream);
                OpacityImage = 1;
            }
        }
        
        if (_audioFileReader != null)
        {
            _totalTime = _audioFileReader.TotalTime;
        }
    }

    private async void PlayPause()
    {
        _isPlaying = true;
        await PlayAudioAsync(_filePath);
        //_audioFileReader.CurrentTime = TimeSpan.FromSeconds(StartSlider.Value);
        _isPlaying = true;
        await PlayAudioAsync(_filePath);
    }
    private async Task PlayAudioAsync(string filePath) 
    { 
        using (var audioFileReader = new AudioFileReader(filePath)) 
        using (var waveOut = new WaveOutEvent()) 
        { 
            waveOut.Init(audioFileReader); 
            waveOut.Play();
            var buffer = new byte[4096]; 
            int bytesRead;
            while (waveOut.PlaybackState == PlaybackState.Playing) 
            {
                bytesRead = await Task.Run(() => audioFileReader.Read(buffer, 0, buffer.Length));
                if (bytesRead == 0) break;
                waveOut.Play();
                await Task.Delay(100); 
            } 
        } 
    }
}