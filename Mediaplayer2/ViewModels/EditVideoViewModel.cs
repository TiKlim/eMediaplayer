using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using NAudio.Lame;
using NAudio.Wave;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class EditVideoViewModel : ViewModelBase, IRoutableViewModel
{
    private string _main;

    private string _premain;

    private string _start;

    private string _end;

    private string _startTime;
    
    private string _endTime;
    
    private TimeSpan _currentTime;
    
    private TimeSpan _endValue;
    
    private Bitmap? volumeImage = new Bitmap("Assets/VolumeOnRed.png");
    
    private Bitmap? playImage = new Bitmap("Assets/ButtonPlayRed.png");
    
    private string _filePath;
    
    private IWavePlayer _waveOut;
    
    private AudioFileReader _audioFileReader;
    
    private TimeSpan _totalTime;
    
    private TimeSpan _audioDuration;
    
    private bool _isPlaying = false;
    
    private float _volume = 1f;
    
    private DispatcherTimer _timer;
    
    private double _endSliderValue;

    private string _endTimeText;
    
    private double _startSliderValue;
    
    private string _startTimeText;
    
    private LibVLC _libVLC;
    
    private MediaPlayer _mediaPlayer;
    
    private VideoView _videoView;
    
    private Mediaplayer2.Models.Equalizer _equalizer;
    
    private readonly SettingsPageViewModel _equalizers;

    private string _save;

    private string _cancel;

    private string _audioFromVideo;
    
    private string _play;
    
    private string _stop;
    
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

    public string Start
    {
        get => _start;
        set => this.RaiseAndSetIfChanged(ref _start, value);
    }

    public string End
    {
        get => _end;
        set => this.RaiseAndSetIfChanged(ref _end, value);
    }

    public string StartTime
    {
        get => _startTime;
        set => this.RaiseAndSetIfChanged(ref _startTime, value);
    }

    public string EndTime
    {
        get => _endTime;
        set => this.RaiseAndSetIfChanged(ref _endTime, value);
    }
    
    public TimeSpan CurrentTime
    {
        get => _currentTime;
        set => this.RaiseAndSetIfChanged(ref _currentTime, value);
    }
    
    public TimeSpan EndValue
    {
        get => _endValue;
        set => this.RaiseAndSetIfChanged(ref _endValue, value);
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

    public double EndSliderValue
    {
        get => _endSliderValue;
        set
        {
            if (_endSliderValue != value)
            {
                _endSliderValue = value;
                this.RaiseAndSetIfChanged(ref _endSliderValue, value);
                EndValue = TimeSpan.FromMilliseconds(_endSliderValue);
                UpdateEndTimeText();
            }
        }
    }

    public string EndTimeText
    {
        get => _endTimeText;
        set
        {
            if (_endTimeText != value)
            {
                _endTimeText = value;
                this.RaiseAndSetIfChanged(ref _endTimeText, value);
            }
        }
    }

    public double StartSliderValue
    {
        get => _startSliderValue;
        set
        {
            if (_startSliderValue != value)
            {
                _startSliderValue = value;
                this.RaiseAndSetIfChanged(ref _startSliderValue, value);
                UpdateStartTimeText();
            }
        }
    }

    public string StartTimeText
    {
        get => _startTimeText;
        set
        {
            if (_startTimeText != value)
            {
                _endTimeText = value;
                this.RaiseAndSetIfChanged(ref _startTimeText, value);
            }
        }
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
    
    public Mediaplayer2.Models.Equalizer Equalizer
    {
        get => _equalizer;
        set => this.RaiseAndSetIfChanged(ref _equalizer, value);
    }
    
    public ICommand PlayPauseCommand { get; }
    
    public ICommand VolumeCommand { get; }
    
    public ICommand BackTime { get; }
    
    public ICommand ForeTime { get; }
    
    public ReactiveCommand<Unit, IRoutableViewModel> SaveCommand { get; }
    
    public ReactiveCommand<Unit, IRoutableViewModel> CancelCommand { get; }
    
    public string? UrlPathSegment => "/editVideo";
    
    public IScreen HostScreen { get; }
    
    public ReactiveCommand<Unit, Unit> ExtractAudio { get; }

    public string SaveBtn
    {
        get => _save;
        set => this.RaiseAndSetIfChanged(ref _save, value);
    }

    public string Cancel
    {
        get => _cancel;
        set => this.RaiseAndSetIfChanged(ref _cancel, value);
    }

    public string AudioFromVideoBtn
    {
        get => _audioFromVideo;
        set => this.RaiseAndSetIfChanged(ref _audioFromVideo, value);
    }
    
    public string Play
    {
        get => _play;
        set => this.RaiseAndSetIfChanged(ref _play, value);
    }

    public string Stop
    {
        get => _stop;
        set => this.RaiseAndSetIfChanged(ref _stop, value);
    }
    
    public EditVideoViewModel()
    {

    }

    public EditVideoViewModel(string filePath, IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        
        Core.Initialize();
        _libVLC = new LibVLC();
        _mediaPlayer = new MediaPlayer(_libVLC);
        
        Main = "Редактор";
        Start = "Начало:";
        End = "Конец:";
        SaveBtn = "Сохранить изменения";
        Cancel = "Отменить изменения";
        AudioFromVideoBtn = "Извлечь аудиодорожку";
        
        Play = "True";
        Stop = "False";
        
        //_equalizer = settingsViewModel.Equalizer;
        
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _timer.Tick += (sender, e) =>
        {
            if (filePath != null && _isPlaying)
            {
                CurrentTime = TimeSpan.FromMilliseconds(_mediaPlayer.Time);
            }
        };
        _timer.Tick += (sender, e) =>
        {
            if (filePath != null && _isPlaying)
            {
                EndValue = TimeSpan.FromMilliseconds(_mediaPlayer.Length);
            }
        };
        
        PreMain = Path.GetFileNameWithoutExtension(filePath);
        
        //AudioDuration = TimeSpan.FromMilliseconds(_mediaPlayer.Length); //*
        
        //EndSliderValue = AudioDuration.TotalMilliseconds;
        
        /*var newTime = TimeSpan.FromMilliseconds(StartSliderValue);
        _mediaPlayer.Time = (int)newTime.TotalMilliseconds;*/
        
        //UpdateStartTimeText();
        //UpdateEndTimeText();
        
        //_audioFileReader.CurrentTime = TimeSpan.FromSeconds(StartSliderValue);
        
        PlayPauseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Debug.WriteLine("PlayPauseCommand executed.");
            if (_isPlaying)
            {
                _mediaPlayer.Pause();
                _timer.Stop();
                _isPlaying = false;
                UpdateVolume();
                //PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                Play = "True";
                Stop = "False";
            }
            else
            {
                Debug.WriteLine($"MediaPlayer State: {_mediaPlayer.State}");
                Debug.WriteLine($"File path: {filePath}");
                if (filePath != null)
                {
                    if (_mediaPlayer.Media == null)
                    {
                        var media = new Media(_libVLC, filePath, FromType.FromPath);
                        _mediaPlayer.Media = media;
                        await Task.Delay(100);
                        Debug.WriteLine("Media assigned to MediaPlayer.");
                        _mediaPlayer.Playing += (sender, e) =>
                        {
                            AudioDuration = TimeSpan.FromMilliseconds(_mediaPlayer.Length);
                            Debug.WriteLine($"Audio Duration: {AudioDuration.TotalSeconds} seconds");
                            EndSliderValue = AudioDuration.TotalMilliseconds;
                            Debug.WriteLine($"Audio Duration: {EndSliderValue} seconds");
                            UpdateEndTimeText();
                        };
                    }
                    _mediaPlayer.Play(); // При частом нажатии на стоп видео может ломаться
                    _timer.Start();
                    _isPlaying = true;
                    UpdateVolume();
                    //PlayImage = new Bitmap("Assets/StopRed.png");
                    Play = "False";
                    Stop = "True";
                }
            }
            
            /*_waveOut.PlaybackStopped += (sender, e) =>
            {
                _isPlaying = false;
                PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                CurrentTime = TimeSpan.Zero;
            };*/
        }, outputScheduler: RxApp.MainThreadScheduler); //*

        VolumeCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (Volume == 0f)
            {
                Volume = 0.5f;
                UpdateVolume();
            }
            else
            {
                Volume = 0f;
                UpdateVolume();
            }
        }, outputScheduler: RxApp.MainThreadScheduler);
        
        BackTime = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_mediaPlayer != null)
            {
                var newTime = TimeSpan.FromMilliseconds(_mediaPlayer.Time) - TimeSpan.FromSeconds(10);
                _mediaPlayer.Time = newTime < TimeSpan.Zero ? 0 : (int)newTime.TotalMilliseconds;
                CurrentTime = TimeSpan.FromMilliseconds(_mediaPlayer.Time);
            }
        }, outputScheduler: RxApp.MainThreadScheduler);

        ForeTime = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_mediaPlayer != null)
            {
                var newTime = TimeSpan.FromMilliseconds(_mediaPlayer.Time) + TimeSpan.FromSeconds(10);
                if (newTime > TimeSpan.FromMilliseconds(_mediaPlayer.Length))
                {
                    newTime = TimeSpan.FromMilliseconds(_mediaPlayer.Length);
                }
                _mediaPlayer.Time = (int)newTime.TotalMilliseconds;
                CurrentTime = TimeSpan.FromMilliseconds(_mediaPlayer.Time);
            }
        }, outputScheduler: RxApp.MainThreadScheduler);

        SaveCommand = ReactiveCommand.CreateFromObservable(() =>
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
            TrimVideoFile(filePath, (double)StartSliderValue, (double)EndSliderValue);
            return HostScreen.Router.Navigate.Execute(new VideoPageViewModel(HostScreen)).ObserveOn(RxApp.MainThreadScheduler);
        });
        
        CancelCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new VideoPageViewModel(HostScreen)).ObserveOn(RxApp.MainThreadScheduler));

        ExtractAudio = ReactiveCommand.CreateFromTask(() => AudioFromVideo(filePath));
    }
    
    public void ApplyEqualizer()
    {
        var currentSettings = _equalizer.CurrentSettings;
    }
    
    

    /*private void Save()
    {
        TrimMp3File(_filePath, (double)StartSliderValue, (double)EndSliderValue);
    }*/
    
    private void UpdateStartTimeText()
    {
        StartTimeText = $"{FormatTime(StartSliderValue)}";
    }

    private void UpdateEndTimeText()
    {
        EndTimeText = $"{FormatTime(EndSliderValue)}";
        Debug.WriteLine($"EndTimeText updated: {EndTimeText}");
    }
    
    /*private string FormatTime(double seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
    }*/
    private string FormatTime(double milliseconds)
    {
        TimeSpan timeSpan = TimeSpan.FromMilliseconds(milliseconds);
        int hours = (int)timeSpan.TotalHours;
        int minutes = (int)timeSpan.TotalMinutes % 60;
        return string.Format("{0:D2}:{1:D2}", hours, minutes);
    }



    
    private void UpdateVolume()
    {
        if (_waveOut != null)
        {
            _waveOut.Volume = Volume;
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
    
    private void TrimVideoFile(string inputFilePath, double startSeconds, double endSeconds)
    {
        string tempFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath)!, $"{Path.GetFileNameWithoutExtension(inputFilePath)}_temp{Path.GetExtension(inputFilePath)}");

        var inputFile = new MediaFile { Filename = inputFilePath };
        var outputFile = new MediaFile { Filename = tempFilePath };
        
        

        using (var engine = new Engine())
        {
            var options = new ConversionOptions
            {
                Seek = TimeSpan.FromSeconds(startSeconds),
                //MaxDuration = TimeSpan.FromSeconds(endSeconds - startSeconds)
            };
            
            engine.Convert(inputFile, outputFile, options);
        }
        
        //File.Delete(inputFilePath);
        //File.Move(tempFilePath, inputFilePath);
        try
        {
            if (File.Exists(inputFilePath))
            {
                File.Delete(inputFilePath);
            }

            File.Move(tempFilePath, inputFilePath);
        }
        catch (IOException ex) // Повторная попытка
        {
            Debug.WriteLine($"Error deleting file: {ex.Message}");
            
            Thread.Sleep(1000);
            TrimVideoFile(inputFilePath, startSeconds, endSeconds);
        }
    }

    private async Task AudioFromVideo(string videoFilePath)
    {
        if (!File.Exists(videoFilePath))
        {
            throw new FileNotFoundException("Video file not found.", videoFilePath);
        }

        // Открытие диалогового окна для выбора места сохранения
        var dialog = new SaveFileDialog
        {
            Title = "Сохранить аудио",
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter
                {
                    Name = "MP3 files",
                    Extensions = { "mp3" }
                }
            }
        };

        var desctop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
        var result = await dialog.ShowAsync(desctop.MainWindow);

        if (string.IsNullOrEmpty(result))
        {
            // Пользователь отменил выбор
            return;
        }

        var inputFile = new MediaFile { Filename = videoFilePath };
        var outputFile = new MediaFile { Filename = result };

        try
        {
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
                engine.Convert(inputFile, outputFile);
            }

            // 
        }
        catch (Exception ex)
        {
            // 
        }
    }
}