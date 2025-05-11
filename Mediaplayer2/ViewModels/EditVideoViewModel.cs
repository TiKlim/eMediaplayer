using System;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
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
    
    private Bitmap? volumeImage = new Bitmap("Assets/VolumeOnRed.png");
    
    private Bitmap? playImage = new Bitmap("Assets/ButtonPlayRed.png");
    
    private string _filePath;
    
    private IWavePlayer _waveOut;
    
    private AudioFileReader _audioFileReader;
    
    private TimeSpan _totalTime;
    
    private TimeSpan _audioDuration;
    
    private bool _isPlaying = false;
    
    private float _volume = 1f;
    
    private System.Timers.Timer _timer;
    
    private double _endSliderValue;

    private string _endTimeText;
    
    private double _startSliderValue;
    
    private string _startTimeText;
    
    private LibVLC _libVLC;
    
    private MediaPlayer _mediaPlayer;
    
    private VideoView _videoView;
    
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
    
    public ICommand PlayPauseCommand { get; }
    
    public ICommand VolumeCommand { get; }
    
    public ICommand BackTime { get; }
    
    public ICommand ForeTime { get; }
    
    public ReactiveCommand<Unit, IRoutableViewModel> SaveCommand { get; }
    
    public ReactiveCommand<Unit, IRoutableViewModel> CancelCommand { get; }
    
    public string? UrlPathSegment => "/editVideo";
    
    public IScreen HostScreen { get; }
    
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
        
        _timer = new System.Timers.Timer(100);
        _timer.Elapsed += (sender, e) =>
        {
            if (filePath != null && _isPlaying)
            {
                CurrentTime = TimeSpan.FromMilliseconds(_mediaPlayer.Time);
                //Debug.WriteLine($"Current Time: {CurrentTime.TotalMilliseconds} seconds");
            }
        };
        
        PreMain = Path.GetFileNameWithoutExtension(filePath);
        
        //EndSliderValue = AudioDuration.TotalSeconds;
        
        //_audioFileReader.CurrentTime = TimeSpan.FromSeconds(StartSliderValue);
        
        PlayPauseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Debug.WriteLine("PlayPauseCommand executed.");
            if (_isPlaying)
            {
                Debug.WriteLine("1");
                _mediaPlayer.Pause();
                Debug.WriteLine("2");
                _timer.Stop();
                Debug.WriteLine("3");
                _isPlaying = false;
                Debug.WriteLine("4");
                UpdateVolume();
                Debug.WriteLine("5");
                PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
            }
            else
            {
                Debug.WriteLine("6");
                Debug.WriteLine($"MediaPlayer State: {_mediaPlayer.State}");
                Debug.WriteLine($"File path: {filePath}");
                if (filePath != null)
                {
                    Debug.WriteLine("7");
                    if (_mediaPlayer.Media == null)
                    {
                        Debug.WriteLine("8");
                        var media = new Media(_libVLC, filePath, FromType.FromPath);
                        _mediaPlayer.Media = media;
                        await Task.Delay(100);
                        Debug.WriteLine("Media assigned to MediaPlayer.");
                        _mediaPlayer.Playing += (sender, e) =>
                        {
                            AudioDuration = TimeSpan.FromMilliseconds(_mediaPlayer.Length);
                            Debug.WriteLine($"Audio Duration: {AudioDuration.TotalSeconds} seconds");
                            EndSliderValue = AudioDuration.TotalMilliseconds;
                        };
                    }
                    Debug.WriteLine($"MediaPlayer State: {_mediaPlayer.State}");
                    _mediaPlayer.Play(); // При частом нажатии на стоп видео может ломаться
                    _timer.Start();
                    _isPlaying = true;
                    UpdateVolume();
                    PlayImage = new Bitmap("Assets/StopRed.png");
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
            TrimVideoFile(filePath, (double)StartSliderValue, (double)EndSliderValue);
            return HostScreen.Router.Navigate.Execute(new VideoPageViewModel(HostScreen)).ObserveOn(RxApp.MainThreadScheduler);
        });
        
        CancelCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new VideoPageViewModel(HostScreen)).ObserveOn(RxApp.MainThreadScheduler));
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
    }
    
    private string FormatTime(double seconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
        return string.Format("{0:D2}:{1:D2}", (int)timeSpan.TotalMinutes, timeSpan.Seconds);
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

        string ffmpegPath = "ffmpeg"; // Убедитесь, что ffmpeg доступен в PATH
        string arguments = $"-i \"{inputFilePath}\" -ss {startSeconds} -to {endSeconds} -c copy \"{tempFilePath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();
        
        if (process.ExitCode != 0)
        {
            string errorOutput = process.StandardError.ReadToEnd();
            throw new Exception($"FFmpeg error: {errorOutput}");
        }
        
        File.Delete(inputFilePath);
        File.Move(tempFilePath, inputFilePath);
    }

}