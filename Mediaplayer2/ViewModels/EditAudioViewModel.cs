using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using NAudio.Lame;
using NAudio.Wave;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class EditAudioViewModel : ViewModelBase, IRoutableViewModel
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
    
    public ICommand PlayPauseCommand { get; }
    
    public ICommand VolumeCommand { get; }
    
    public ICommand BackTime { get; }
    
    public ICommand ForeTime { get; }
    
    public ReactiveCommand<Unit, IRoutableViewModel> SaveCommand { get; }
    
    public ReactiveCommand<Unit, IRoutableViewModel> CancelCommand { get; }
    
    public string? UrlPathSegment => "/editAudio";
    
    public IScreen HostScreen { get; }
    
    public EditAudioViewModel()
    {

    }

    public EditAudioViewModel(string filePath, IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        Main = "Редактор";
        
        Start = "Начало:";
        End = "Конец:";
        
        _timer = new System.Timers.Timer(100); // Обновление каждые 100 мс
        _timer.Elapsed += (sender, e) =>
        {
            if (_audioFileReader != null && _isPlaying)
            {
                CurrentTime = _audioFileReader.CurrentTime;
            }
        };
        
        var file = TagLib.File.Create(filePath);
        string title = file.Tag.Title ?? "Нет названия";
        string performer = file.Tag.Performers.Length > 0 ? file.Tag.Performers[0] : "Нет исполнителя";
        
        PreMain = $"{title} | {performer}";
        
        if (_audioFileReader != null)
        {
            _totalTime = _audioFileReader.TotalTime;
            AudioDuration = _totalTime;
        }
                
        // Освобождение предыдущего ресурса, если он существует
        _audioFileReader?.Dispose();
        _waveOut?.Dispose();
                
        _audioFileReader = new AudioFileReader(filePath);
        
        _waveOut = new WaveOutEvent();
        _waveOut.Init(_audioFileReader); 
        AudioDuration = _audioFileReader.TotalTime;
        EndSliderValue = AudioDuration.TotalSeconds;
        
        _audioFileReader.CurrentTime = TimeSpan.FromSeconds(StartSliderValue);
        
        PlayPauseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_isPlaying)
            {
                _waveOut?.Pause();
                _timer.Stop();
                _isPlaying = false;
                UpdateVolume();
                PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
            }
            else
            {
                // Запустить воспроизведение
                if (_audioFileReader != null)
                {
                    // Убедитесь, что WaveOut инициализирован
                    if (_waveOut == null)
                    {
                        _waveOut = new WaveOutEvent();
                        _waveOut.Init(_audioFileReader);
                    }

                    _waveOut.Volume = Volume; // Установка громкости
                    _waveOut.Play(); // Запуск воспроизведения
                    _timer.Start();
                    _isPlaying = true;
                    PlayImage = new Bitmap("Assets/StopRed.png");
                }
            }
            
            _waveOut.PlaybackStopped += (sender, e) =>
            {
                _isPlaying = false;
                PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                CurrentTime = TimeSpan.Zero;
            };
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

        SaveCommand = ReactiveCommand.CreateFromObservable(() =>
        {
            TrimMp3File(filePath, (double)StartSliderValue, (double)EndSliderValue);
            return HostScreen.Router.Navigate.Execute(new MusicPageViewModel(HostScreen)).ObserveOn(RxApp.MainThreadScheduler);
        });
        
        CancelCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new MusicPageViewModel(HostScreen)).ObserveOn(RxApp.MainThreadScheduler));
    }
    
    

    private void Save()
    {
        TrimMp3File(_filePath, (double)StartSliderValue, (double)EndSliderValue);
    }
    
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
    
    private void TrimMp3File(string inputFilePath, double startSeconds, double endSeconds)
    {
            
        string tempFilePath = Path.Combine(Path.GetDirectoryName(inputFilePath)!, $"{Path.GetFileNameWithoutExtension(inputFilePath)}_temp.mp3");

        using (var reader = new AudioFileReader(inputFilePath))
        {
                
            reader.CurrentTime = TimeSpan.FromSeconds(startSeconds);
            using (var writer = new LameMP3FileWriter(tempFilePath, reader.WaveFormat, LAMEPreset.STANDARD))
            {
                var buffer = new byte[4096];
                int bytesRead;
                TimeSpan endTime = TimeSpan.FromSeconds(endSeconds);

                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (reader.CurrentTime >= endTime)
                        break;

                    writer.Write(buffer, 0, bytesRead);
                }
            }
        }

            
        File.Delete(inputFilePath);
            
        File.Move(tempFilePath, inputFilePath);
    }
}