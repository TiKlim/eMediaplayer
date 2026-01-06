using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Mediaplayer2.Models;
using Mediaplayer2.Views;
using NAudio.Wave;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class MusicFromListViewModel : ViewModelBase, IRoutableViewModel, IDisposable, IActivatableViewModel
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
    
    //private System.Timers.Timer _timer;
    
    private IRoutableViewModel _routableViewModelImplementation;

    private string _visibleAttention;
    
    private string _visibleImage;

    private string _attention;

    //private readonly SettingsPageViewModel _equalizer;
    
    private Equalizer _equalizer;
    
    private readonly SettingsPageViewModel _equalizers;
    
    private EqualizerSampleProvider _equalizerProvider;
    
    private AudioSettings _audioSettings;
    
    private string _selectedPreset = "Normal";

    private DispatcherTimer _timer;

    private string _pop;

    private string _vocal;
    
    private string _rock;

    private string _jazz;
    
    private string _classical;

    private string _bass;

    private string _loadFIle;
    
    private List<string> _playlist = new List<string>();
    
    private int _currentTrackIndex = -1;
    
    private bool _canGoBack => _currentTrackIndex > 0;
    
    private bool _canGoForward => _currentTrackIndex < _playlist.Count - 1;

    private string _volumeOn;
    
    private string _volumeOff;
    
    private string _play;
    
    private string _stop;

    private string _trackImg;
    
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
    
    public ICommand LoadFileCommand { get; }
    
    public ICommand PlayPauseCommand { get; }
    
    public ICommand VolumeCommand { get; }
    
    public ICommand BackTime { get; }
    
    public ICommand ForeTime { get; }

    public string? UrlPathSegment => "/list";
    public IScreen HostScreen { get; }
    
    public ReactiveCommand<Unit, IRoutableViewModel> ToEditAudioPageCommand { get; }

    public string VisibleAttention
    {
        get => _visibleAttention;
        set => this.RaiseAndSetIfChanged(ref _visibleAttention, value);
    }

    public string VisibleImage
    {
        get => _visibleImage;
        set => this.RaiseAndSetIfChanged(ref _visibleImage, value);
    }

    public string Attention { get; } = "1. Добавьте треки\n2. Вернитесь на страницу с плейлистами\n3. Файлы сохранились, можете выбирать плейлист";
    
    public Dictionary<string, float[]> EqualizerPresets { get; } = new Dictionary<string, float[]>
    {
        { "Pop", new float[10] { 0.5f, 1.0f, 0.8f, 0.5f, 0.3f, 0.2f, 0.2f, 0.2f, 0.2f, 0.2f } },
        { "Vocal", new float[10] { 0f, 1f, 2f, 3f, 2f, 1f, 0f, 0f, 0f, 0f } },
        { "Rock", new float[10] { 0.7f, 0.9f, 1.0f, 0.6f, 0.4f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f } },
        { "Jazz", new float[10] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f } },
        { "Classical", new float[10] { 0.6f, 0.7f, 0.8f, 0.9f, 0.7f, 0.6f, 0.6f, 0.6f, 0.6f, 0.6f } },
        { "Bass Boost", new float[10] { 5f, 4f, 3f, 2f, 1f, 0f, 0f, 0f, 0f, 0f } }
    };
    
    public string SelectedPreset
    {
        get => _selectedPreset;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedPreset, value);
            ApplyPreset(value);
        }
    }
    
    public ReactiveCommand<string, Unit> SelectPresetCommand { get; }

    public string Pop
    {
        get => _pop;
        set => this.RaiseAndSetIfChanged(ref _pop, value);
    }

    public string Vocal
    {
        get => _vocal;
        set => this.RaiseAndSetIfChanged(ref _vocal, value);
    }

    public string Rock
    {
        get => _rock;
        set => this.RaiseAndSetIfChanged(ref _rock, value);
    }

    public string Jazz
    {
        get => _jazz;
        set => this.RaiseAndSetIfChanged(ref _jazz, value);
    }

    public string Classical
    {
        get => _classical;
        set => this.RaiseAndSetIfChanged(ref _classical, value);
    }

    public string Bass
    {
        get => _bass;
        set => this.RaiseAndSetIfChanged(ref _bass, value);
    }

    public string LoadFile
    {
        get => _loadFIle;
        set => this.RaiseAndSetIfChanged(ref _loadFIle, value);
    }
    
    public ICommand OpenEqualizerMenuCommand { get; }
    
    public ICommand PreviousTrackCommand { get; }
    public ICommand NextTrackCommand { get; }

    public string VolumeOn
    {
        get => _volumeOn;
        set => this.RaiseAndSetIfChanged(ref _volumeOn, value);
    }

    public string VolumeOff
    {
        get => _volumeOff;
        set => this.RaiseAndSetIfChanged(ref _volumeOff, value);
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

    public string TrackImg
    {
        get => _trackImg;
        set => this.RaiseAndSetIfChanged(ref _trackImg, value);
    }
    
    public void StopPlayback()
    {
        if (_isPlaying)
        {
            _waveOut?.Stop();
            _audioFileReader?.Dispose();
            _waveOut?.Dispose();
            _isPlaying = false;
            CurrentTime = TimeSpan.Zero; // Сброс текущего времени
        }
    }
    
    public void Dispose()
    {
        _waveOut?.Stop();
        _audioFileReader?.Dispose();
        _waveOut?.Dispose();
        _isPlaying = false;
    }
    
    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    public MusicFromListViewModel()
    {

    }

    public MusicFromListViewModel(Playlist playlist, IScreen? screen = null)
    {
        if (playlist == null)
            throw new ArgumentNullException(nameof(playlist));
        
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        // Останавливаем плеер при переключении на другую страницу
        this.WhenActivated(disposables =>
        {
            Disposable.Create(() =>
            {
                StopPlayback();
                Dispose();
            }).DisposeWith(disposables);
        });
        
        Main = playlist.Name ?? "Без названия";
        PreMain = "Добавление треков в плейлист";
        Pop = "Поп"; 
        Vocal = "Вокал";
        Rock = "Рок";
        Jazz = "Джаз";
        Classical = "Классический";
        Bass = "Усиление низких частот";
        LoadFile = "Добавить трек";
        //TrackImage = new Bitmap("Assets/ListStopRed.png");
        OpacityImage = 0.2;
        VisibleImage = "true";
        VisibleAttention = "false";

        VolumeOn = "True";
        VolumeOff = "False";
        
        Play = "True";
        Stop = "False";

        TrackImg = "False";
        
        _equalizer = new Equalizer();
        //_audioSettings = audioSettings;
        
        ApplyEqualizer();
        
        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(100)
        };
        _timer.Tick += (sender, e) =>
        {
            if (_audioFileReader != null && _isPlaying)
            {
                CurrentTime = _audioFileReader.CurrentTime;
            }
        };
        
        LoadFileCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    AllowMultiple = true,
                    Filters = new List<FileDialogFilter>
                    {
                        new FileDialogFilter { Name = "MP3 Files", Extensions = { "mp3" } }
                    }
                };
                var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
                var result = await dialog.ShowAsync(desktop.MainWindow);
                if (result.Length > 0)
                {
                    foreach (var file in result)
                    {
                        // Добавляем трек в плейлист
                        playlist.AddTrack(file); // Используем метод для добавления трека
                    }

                    // Если сейчас ничего не играет, запустим первый трек из добавленных
                    if (!_isPlaying && playlist.Tracks.Count > 0)
                    {
                        _currentTrackIndex = 0;
                        await PlayTrackAtIndex(_currentTrackIndex);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
            }
        }, outputScheduler: RxApp.MainThreadScheduler);

        PlayPauseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_playlist.Count == 0)
            {
                VisibleImage = "false";
                VisibleAttention = "true";
                await Task.Delay(5000);
                VisibleImage = "true";
                VisibleAttention = "false";
                return;
            }

            if (_isPlaying)
            {
                _waveOut?.Pause();
                _timer.Stop();
                _isPlaying = false;
                //PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                Play = "True";
                Stop = "False";
            }
            else
            {
                if (_waveOut == null && _playlist.Count > 0)
                {
                    _currentTrackIndex = _currentTrackIndex == -1 ? 0 : _currentTrackIndex;
                    await PlayTrackAtIndex(_currentTrackIndex);
                }
                else
                {
                    _waveOut?.Play();
                    _timer.Start();
                    Debug.WriteLine($"Current Track Index: {_currentTrackIndex}");
                    Debug.WriteLine($"Количество треков в плейлисте: {_playlist.Count}"); //*
                    _isPlaying = true;
                    //PlayImage = new Bitmap("Assets/StopRed.png");
                    Play = "False";
                    Stop = "True";
                }
            }
        }, outputScheduler: RxApp.MainThreadScheduler);

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
        
        PreviousTrackCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_currentTrackIndex > 0)
            {
                _currentTrackIndex--; // Переход к предыдущему треку
                await PlayTrackAtIndex(_currentTrackIndex); // Воспроизводим трек
                Debug.WriteLine($"Current Track Index: {_currentTrackIndex}");
            }
        }, outputScheduler: RxApp.MainThreadScheduler);
    
        NextTrackCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_currentTrackIndex < _playlist.Count - 1)
            {
                _currentTrackIndex++; // Переход к следующему треку
                await PlayTrackAtIndex(_currentTrackIndex); // Воспроизводим трек
                Debug.WriteLine($"Current Track Index: {_currentTrackIndex}");
            }
        }, outputScheduler: RxApp.MainThreadScheduler);
        
        ToEditAudioPageCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new EditAudioViewModel(_filePath, HostScreen)).ObserveOn(RxApp.MainThreadScheduler));
        
        SelectPresetCommand = ReactiveCommand.Create<string>(presetName =>
        {
            SelectedPreset = presetName; // Устанавливаем выбранный пресет
            ApplyPreset(presetName); // Применяем пресет
        }, outputScheduler: RxApp.MainThreadScheduler);
        
        OpenPlaylist(playlist);
    }
    
    /*private void UpdateTrackNavigation()
    {
        OnPropertyChanged(nameof(CanGoBack));
        OnPropertyChanged(nameof(CanGoForward));
    }*/
    
    public async Task OpenPlaylist(Playlist playlist)
    {
        if (Directory.Exists(playlist.FolderPath))
        {
            _playlist.Clear(); // Очищаем текущий плейлист
            _playlist.AddRange(Directory.GetFiles(playlist.FolderPath, "*.mp3")); // Получаем все mp3 файлы в папке

            // Если плейлист не пуст, начинаем воспроизведение
            if (_playlist.Count > 0)
            {
                _currentTrackIndex = 0; // Начинаем с первого трека
                await PlayTrackAtIndex(_currentTrackIndex);
            }
        }
        else
        {
            Debug.WriteLine("Папка плейлиста не найдена.");
        }
    }
    
    private async Task PlayTrackAtIndex(int index)
    {
        if (index < 0 || index >= _playlist.Count)
            return;

        var filePath = _playlist[index];

        // Освободить предыдущие ресурсы
        _audioFileReader?.Dispose();
        _waveOut?.Dispose();

        _audioFileReader = new AudioFileReader(filePath);

        // Подготовка эквалайзера, если нужно
        _equalizerProvider = new EqualizerSampleProvider(_audioFileReader, _equalizer.CurrentSettings);

        _waveOut = new WaveOutEvent();
        _waveOut.Init(_equalizerProvider);

        _waveOut.Volume = Volume;

        LoadMp3Info(filePath);

        _waveOut.PlaybackStopped += OnPlaybackStopped;

        _waveOut.Play();
        _isPlaying = true;
        //PlayImage = new Bitmap("Assets/StopRed.png");
        Play = "False";
        Stop = "True";

        AudioDuration = _audioFileReader.TotalTime;

        _timer.Start();
    }
    
    private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
    {
        _waveOut.PlaybackStopped -= OnPlaybackStopped;

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            _isPlaying = false;
            //PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
            Play = "True";
            Stop = "False";
            CurrentTime = TimeSpan.Zero;

            _currentTrackIndex++;

            if (_currentTrackIndex < _playlist.Count)
            {
                await PlayTrackAtIndex(_currentTrackIndex);
            }
            else
            {
                // Дошли до конца плейлиста
                _currentTrackIndex = -1;
                _timer.Stop();
            }
        });
    }
    
    // Метод для применения эквалайзера
    private void ApplyPreset(string presetName)
    {
        if (EqualizerPresets.TryGetValue(presetName, out var gains) && _equalizerProvider != null)
        {
            for (int i = 0; i < gains.Length; i++)
            {
                _equalizerProvider.Gains[i] = gains[i];
            }
            UpdateEqualizer(gains); // Обновляем эквалайзер с новыми значениями
        }
        else
        {
            Debug.WriteLine($"Пресет '{presetName}' не найден или эквалайзер не инициализирован.");
        }
    }
    
    public void UpdateEqualizer(float[] newGains)
    {
        if (_equalizerProvider != null)
        {
            for (int i = 0; i < newGains.Length && i < _equalizerProvider.Gains.Length; i++)
            {
                _equalizerProvider.Gains[i] = newGains[i];
            }
        }
    }
    
    private void ApplyEqualizer()
    {
        if (_equalizerProvider != null && _equalizer.CurrentSettings != null)
        {
            for (int i = 0; i < _equalizer.CurrentSettings.Length && i < _equalizerProvider.Gains.Length; i++)
            {
                _equalizerProvider.Gains[i] = _equalizer.CurrentSettings[i];
            }
        }
    }
    
    private void LoadMp3Info(string filePath)
    {
        try
        {
            var file = TagLib.File.Create(filePath);
            string title = file.Tag.Title ?? "Нет названия";
            string performer = file.Tag.Performers.Length > 0 ? file.Tag.Performers[0] : "Нет исполнителя";
    
            //Main = performer;    // Исполнитель
            PreMain = performer + " | " + title;     // Название трека
    
            if (file.Tag.Pictures.Length > 0)
            {
                var picture = file.Tag.Pictures[0];
                using (var stream = new MemoryStream(picture.Data.Data))
                {
                    TrackImage = new Bitmap(stream);
                    VisibleImage = "false";
                    TrackImg = "True";
                    OpacityImage = 1;
                }
            }
    
            if (_audioFileReader != null)
            {
                _totalTime = _audioFileReader.TotalTime;
                AudioDuration = _totalTime;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Ошибка при загрузке MP3 информации: {ex.Message}");
        }
    }


    private void UpdateVolume()
    {
        if (_waveOut != null)
        {
            _waveOut.Volume = Volume;
        }

        if (Volume == 0f)
        {
            VolumeOn = "False";
            VolumeOff = "True";
        }
        else
        {
            VolumeOn = "True";
            VolumeOff = "False";
        }
    }
}