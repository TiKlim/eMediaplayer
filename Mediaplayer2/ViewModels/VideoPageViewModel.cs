using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
using NAudio.Wave;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class VideoPageViewModel : ViewModelBase, IRoutableViewModel
{
    private string _main;

    private string _premain;

    private Bitmap? trackImage;
    
    private double _opacityImage;

    private TimeSpan _currentTime;
    
    private Bitmap? volumeImage = new Bitmap("Assets/VolumeOnRed.png");
    
    private Bitmap? playImage = new Bitmap("Assets/ButtonPlayRed.png");
    
    private string _filePath;
    
    private IWavePlayer _waveOut;
    
    private AudioFileReader _audioFileReader;
    
    private TimeSpan _totalTime;
    
    private TimeSpan _audioDuration;
    
    private bool _isPlaying = false;
    
    private float _volume = 1f;
    
    private LibVLC _libVLC;
    
    private MediaPlayer _mediaPlayer;
    
    private VideoView _videoView;

    private bool _visible;
    
    private System.Timers.Timer _timer;

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

    public TimeSpan CurrentTime
    {
        get => _currentTime;
        set => this.RaiseAndSetIfChanged(ref _currentTime, value);
    }

    public Bitmap? VolumeImage
    {
        get => volumeImage;
        set => this.RaiseAndSetIfChanged(ref volumeImage, value);
    }

    public Bitmap? PlayImage
    {
        get => playImage;
        set => this.RaiseAndSetIfChanged(ref playImage, value);
    }

    public double Value { get; set; }

    public float Volume 
    {
        get => _volume;
        set
        {
            if (value != _volume)
            {
                _volume = value;
                this.RaiseAndSetIfChanged(ref _volume, value);
                UpdateVolume();
            }
        }
    }

    public TimeSpan AudioDuration
    {
        get => _audioDuration;
        set => this.RaiseAndSetIfChanged(ref _audioDuration, value);
    }

    public string FilePath
    {
        get => _filePath;
        set => this.RaiseAndSetIfChanged(ref _filePath, value);
    }

    public MediaPlayer? MediaPlayer
    {
        get => _mediaPlayer;
        set => this.RaiseAndSetIfChanged(ref _mediaPlayer, value);
    }

    public bool Visible
    {
        get => _visible;
        set => this.RaiseAndSetIfChanged(ref _visible, value);
    }
    
    public ICommand LoadFileCommand { get; }
    
    public RoutingState Router { get; } = new RoutingState();
    
    public ReactiveCommand<Unit, IRoutableViewModel> ToHome { get; }
    
    public ICommand PlayPauseCommand { get; }
    
    public ICommand VolumeCommand { get; }
    
    public ICommand BackTime { get; }
    
    public ICommand ForeTime { get; }
    
    public event Action<MediaPlayer> OnPlay;
    
    //public double VideoDuration => _mediaPlayer.Length > 0 ? _mediaPlayer.Length / 1000.0 : 0;
    
    public string? UrlPathSegment => "/music";
    
    public IScreen HostScreen { get; }

    public VideoPageViewModel()
    {
        
    }

    public VideoPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        
        Core.Initialize();
        _libVLC = new LibVLC();
        _mediaPlayer = new MediaPlayer(_libVLC);

        Main = "Видеоплеер";
        PreMain = "Что посмотрим сегодня?";
        TrackImage = new Bitmap("Assets/VideoPagePictureRed.png");
        OpacityImage = 0.2;
        Visible = false;
        
        /*_timer = new System.Timers.Timer(100); // Обновление каждые 100 мс
        _timer.Elapsed += (sender, e) =>
        {
            if (_audioFileReader != null && _isPlaying)
            {
                CurrentTime = _audioFileReader.CurrentTime;
            }
        };*/
        
        LoadFileCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "Video Files", Extensions = { "mp4", "mkv", "avi" } }
                }
            };
            var desctop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            var result = await dialog.ShowAsync(desctop.MainWindow);
            if (result.Length > 0)
            {
                _filePath = result[0];
                LoadVideoInfo(_filePath);
                
                //_audioFileReader?.Dispose();
                //_mediaPlayer?.Dispose();
                
                /*_libVLC = new LibVLC();
                _mediaPlayer = new MediaPlayer(_libVLC);
                AudioDuration = _audioFileReader.TotalTime;*/
                
                PlayVideoAsync(_filePath);
                //_audioFileReader = new AudioFileReader(_filePath);
                //AudioDuration = new TimeSpan(_mediaPlayer.Time);
                //_audioDuration = _audioFileReader.TotalTime; 
                //Value = _audioDuration.TotalSeconds;
                //_waveOut = new WaveOutEvent();
                //_waveOut.Init(_audioFileReader); 
            }
        });

        PlayPauseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_isPlaying)
            {
                _mediaPlayer.Pause();
                _isPlaying = false;
                PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
            }
            else
            {
                _mediaPlayer.Play();
                _isPlaying = true;
                PlayImage = new Bitmap("Assets/StopRed.png");
                await PlayVideoAsync(_filePath);
            }
            
            /*if (_isPlaying)
            {
                //_waveOut?.Init(_audioFileReader);
                //_waveOut.Volume = Volume;
                _mediaPlayer.Pause();
                _timer.Stop();
                _isPlaying = false;
                UpdateVolume();
                //CurrentTime = _audioFileReader.CurrentTime; // Обновляем текущее время
                PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
            }
            else
            {
                // Запустить воспроизведение
                if (_audioFileReader != null)
                {
                    // Убедитесь, что WaveOut инициализирован
                    if (_mediaPlayer == null)
                    {
                        _libVLC = new LibVLC();
                        _mediaPlayer = new MediaPlayer(_libVLC);
                    }

                    //_mediaPlayer.Volume = Volume; // Установка громкости
                    _mediaPlayer.Play(); // Запуск воспроизведения
                    _timer.Start();
                    _isPlaying = true;
                    PlayImage = new Bitmap("Assets/StopRed.png");
                }
                //await PlayAudioAsync(_filePath);
            }*/
            
            /*_mediaPlayer.PlaybackStopped += (sender, e) =>
            {
                _isPlaying = false;
                PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                CurrentTime = TimeSpan.Zero;
            };*/
        }, outputScheduler: RxApp.MainThreadScheduler);

        VolumeCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (Volume == 0f)
            {
                Volume = 0.5f;
                UpdateVolume();
                //VolumeImage = new Bitmap("Assets/VolumeOnRed.png");
            }
            else
            {
                Volume = 0f;
                UpdateVolume();
                //VolumeImage = new Bitmap("Assets/VolumeOffRed.png");
            }
        });
        
        BackTime = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_audioFileReader != null)
            {
                var newTime = _audioFileReader.CurrentTime - TimeSpan.FromSeconds(10);
                _audioFileReader.CurrentTime = newTime < TimeSpan.Zero ? TimeSpan.Zero : newTime;
                CurrentTime = _audioFileReader.CurrentTime;
            }
        }, outputScheduler: RxApp.MainThreadScheduler);

        ForeTime = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_audioFileReader != null)
            {
                var newTime = _audioFileReader.CurrentTime + TimeSpan.FromSeconds(10);
                _audioFileReader.CurrentTime = newTime > _audioFileReader.TotalTime ? _audioFileReader.TotalTime : newTime;
                CurrentTime = _audioFileReader.CurrentTime;
            }
        }, outputScheduler: RxApp.MainThreadScheduler);
    }
    
    private void LoadVideoInfo(string filePath)
    {
        Main = Path.GetFileNameWithoutExtension(filePath);
        PreMain = Path.GetFileNameWithoutExtension(filePath);
        //OpacityImage = 1;
        Visible = true;
        /*var file = TagLib.File.Create(filePath);

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
        }*/

        /*if (_mediaPlayer != null)
        {
            _totalTime = new TimeSpan(_mediaPlayer.Time);
            AudioDuration = _totalTime;
            Debug.WriteLine($"Audio Duration: {AudioDuration.TotalSeconds} seconds");
        }*/
    }

    /*private async void PlayPause()
    {
        _isPlaying = true;
        await PlayVideoAsync(_filePath);
        //_audioFileReader.CurrentTime = TimeSpan.FromSeconds(StartSlider.Value);
        _isPlaying = true;
        await PlayVideoAsync(_filePath);
    }*/

    private void UpdateVolume()
    {
        if (_mediaPlayer != null)
        {
            _mediaPlayer.Volume = (int)(Volume * 100);
        }

        if (Volume == 0f)
        {
            VolumeImage = new Bitmap("Assets/VolumeOffRed.png");
        }
        else
        {
            VolumeImage = new Bitmap("Assets/VolumeOnRed.png");
        }
    }
    
    private async Task PlayVideoAsync(string filePath)
    {
        var media = new Media(_libVLC, filePath, FromType.FromPath);
        _mediaPlayer.Media = media;
        
        _mediaPlayer.Playing += (sender, e) =>
        {
            AudioDuration = TimeSpan.FromMilliseconds(_mediaPlayer.Length);
            Debug.WriteLine($"Audio Duration: {AudioDuration.TotalSeconds} seconds");
        };
        
        _mediaPlayer.Play(media);
        
        //AudioDuration = TimeSpan.FromMilliseconds(_mediaPlayer.Length);
        //Debug.WriteLine($"Audio Duration: {AudioDuration.TotalSeconds} seconds");
        
        _isPlaying = true;

        while (_isPlaying)
        {
            await Task.Delay(100);
            if (_mediaPlayer.State == VLCState.Playing)
            {
                CurrentTime = TimeSpan.FromMilliseconds(_mediaPlayer.Time); 
                //Debug.WriteLine($"Current Time: {CurrentTime.TotalSeconds} seconds");
            }
            else
            {
                _isPlaying = false;
            }
        }
        
        //_videoView.MediaPlayer = MediaPlayer;
        UpdateVolume();
        
        /*using (var audioFileReader = new AudioFileReader(filePath)) 
        using (var waveOut = new WaveOutEvent()) 
        { 
            waveOut.Init(audioFileReader); 
            waveOut.Volume = Volume;
            UpdateVolume();
            waveOut.Play();
            var buffer = new byte[2096]; //4096 
            int bytesRead;
            while (waveOut.PlaybackState == PlaybackState.Playing) 
            {
                bytesRead = await Task.Run(() => audioFileReader.Read(buffer, 0, buffer.Length));
                if (bytesRead == 0) break;
                //waveOut.Write(buffer, 0, bytesRead);
                await Task.Delay(100); 
                CurrentTime = audioFileReader.CurrentTime;
                //await Task.Delay(10); 
            }

            if (waveOut.PlaybackState == PlaybackState.Stopped || waveOut.PlaybackState == PlaybackState.Paused)
            {
                _isPlaying = false;
                PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
            }
        }*/
        
        
    }
}