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
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Mediaplayer2.Models;
using Mediaplayer2.Views;
using NAudio.Wave;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class MusicPageViewModel : ViewModelBase, IRoutableViewModel
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

    public string? UrlPathSegment => "/music";
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

    public string Attention { get; } = "Выберите файл";
    
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

    public MusicPageViewModel()
    {

    }

    public MusicPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        Main = "Аудиоплеер";
        PreMain = "Что послушаем сегодня?";
        Pop = "Поп";
        Vocal = "Вокал";
        Rock = "Рок";
        Jazz = "Джаз";
        Classical = "Классический";
        Bass = "Усиление низких частот";
        LoadFile = "Найти трек";
        TrackImage = new Bitmap("Assets/MusicPagePictureRed2.png");
        OpacityImage = 0.2;
        VisibleImage = "true";
        VisibleAttention = "false";
        
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

                    // Освобождение предыдущего ресурса, если он существует
                    _audioFileReader?.Dispose();
                    _waveOut?.Dispose();

                    _audioFileReader = new AudioFileReader(_filePath);
                    
                    _waveOut = new WaveOutEvent();
                    _waveOut.Init(_audioFileReader);
        
                    // Проверка эквалайзера перед инициализацией
                    if (_equalizer.CurrentSettings == null || _equalizer.CurrentSettings.Length != 10)
                    {
                        Debug.WriteLine("Equalizer CurrentSettings: ");
                        foreach (var setting in _equalizer.CurrentSettings)
                        {
                            //Debug.WriteLine($"Band: {setting.Frequency}, Gain: {setting.Gain}, Q: {setting.Q}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("Equalizer CurrentSettings is null.");
                    }
                    
                    _equalizerProvider = new EqualizerSampleProvider(_audioFileReader, _equalizer.CurrentSettings);
                    Debug.WriteLine($"Смотри: {_equalizerProvider}");
                    
                    _waveOut.Init(_equalizerProvider);
                    
                    AudioDuration = _audioFileReader.TotalTime;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
            }
        }, outputScheduler: RxApp.MainThreadScheduler);

        PlayPauseCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            if (_filePath == null)
            {
                VisibleImage = "false";
                VisibleAttention = "true";
                await Task.Delay(2000);
                VisibleImage = "true";
                VisibleAttention = "false";
            }
            else
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
                        Debug.WriteLine($"Ошибка здесь (1)");
                        if (_waveOut == null)
                        {
                            _waveOut = new WaveOutEvent();
                            _waveOut.Init(_audioFileReader);
                            _waveOut.Init(_equalizerProvider); // _audioFileReader
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
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        _isPlaying = false;
                        PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                        CurrentTime = TimeSpan.Zero;
                    });
                };
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
        
        ToEditAudioPageCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new EditAudioViewModel(_filePath, HostScreen)).ObserveOn(RxApp.MainThreadScheduler));
        
        SelectPresetCommand = ReactiveCommand.Create<string>(presetName =>
        {
            SelectedPreset = presetName; // Устанавливаем выбранный пресет
            ApplyPreset(presetName); // Применяем пресет
        }, outputScheduler: RxApp.MainThreadScheduler);
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
            VolumeImage = new Bitmap("Assets/VolumeOffRed.png");
        }
        else
        {
            VolumeImage = new Bitmap("Assets/VolumeOnRed.png");
        }
    }
}