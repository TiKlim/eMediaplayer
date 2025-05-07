using System;
using System.Windows.Input;
using Avalonia.Media.Imaging;
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
    
    public ICommand PlayPauseCommand { get; }
    
    public ICommand VolumeCommand { get; }
    
    public ICommand BackTime { get; }
    
    public ICommand ForeTime { get; }
    
    public string? UrlPathSegment => "/editAudio";
    
    public IScreen HostScreen { get; }
    
    public EditAudioViewModel()
    {

    }

    public EditAudioViewModel(string filePath, IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        Main = "Редактор";
        
        var file = TagLib.File.Create(filePath);
        string title = file.Tag.Title ?? "Нет названия";
        string performer = file.Tag.Performers.Length > 0 ? file.Tag.Performers[0] : "Нет исполнителя";
        
        PreMain = $"{title} | {performer}";
        Start = "Начало:";
        End = "Конец:";
        
        _timer = new System.Timers.Timer(100); // Обновление каждые 100 мс
        _timer.Elapsed += (sender, e) =>
        {
            if (_audioFileReader != null && _isPlaying)
            {
                //Dispatcher.UIThread.InvokeAsync(() => { CurrentTime = _audioFileReader.CurrentTime; });
                CurrentTime = _audioFileReader.CurrentTime;
            }
        };
                
        // Освобождение предыдущего ресурса, если он существует
        _audioFileReader?.Dispose();
        _waveOut?.Dispose();
                
        _audioFileReader = new AudioFileReader(_filePath);
                
        //_audioDuration = _audioFileReader.TotalTime; 
        //Value = _audioDuration.TotalSeconds;
        _waveOut = new WaveOutEvent();
        _waveOut.Init(_audioFileReader); 
        AudioDuration = _audioFileReader.TotalTime;
        
        PlayPauseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_isPlaying)
            {
                //_waveOut?.Init(_audioFileReader);
                //_waveOut.Volume = Volume;
                _waveOut?.Pause();
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
                //await PlayAudioAsync(_filePath);
            }
            
            _waveOut.PlaybackStopped += (sender, e) =>
            {
                //Dispatcher.UIThread.InvokeAsync(() =>
                //{
                    _isPlaying = false;
                    PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                    CurrentTime = TimeSpan.Zero;
                //});
            };
        }, outputScheduler: RxApp.MainThreadScheduler); //*

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
}