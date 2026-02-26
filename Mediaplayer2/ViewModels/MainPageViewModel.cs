using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Mediaplayer2.Models;
using NAudio.Wave;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class MainPageViewModel : ReactiveObject, IRoutableViewModel
{
    private const string SettingsFileName = "userSettings.json";

    private string GetSettingsFilePath()
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(folder, "Mediaplayer");
        if (!Directory.Exists(appFolder))
            Directory.CreateDirectory(appFolder);
        return Path.Combine(appFolder, SettingsFileName);
    }
    
    private Theme _selectedTheme;
    
    private Language _selectedLanguage;
    
    private string _play;
    
    private string _stop;
    
    private string _loadFIle;
    
    private string _volumeOn;
    
    private string _volumeOff;

    private string _trackImg;

    private string _coverImg;
    
    private DispatcherTimer _timer;
    
    private string _visibleAttention;
    
    private string _visibleImage;
    
    private Bitmap? trackImage;
    
    private double _opacityImage;

    private TimeSpan _currentTime;
    
    private Bitmap? volumeImage = new Bitmap("Assets/VolumeOnRed.png");
    
    private Bitmap? playImage = new Bitmap("Assets/FocusRed.png");
    
    private string _filePath;
    
    private IWavePlayer _waveOut;
    
    private AudioFileReader _audioFileReader;
    
    private TimeSpan _totalTime;
    
    private TimeSpan _audioDuration;
    
    private bool _isPlaying = false;
    
    private float _volume = 1f;
    
    private Equalizer _equalizer;
    
    private readonly MainPageViewModel _equalizers;
    
    private EqualizerSampleProvider _equalizerProvider;
    
    private string _pop;

    private string _vocal;
    
    private string _rock;

    private string _jazz;
    
    private string _classical;

    private string _bass;
    
    private string _selectedPreset = "Normal";
    
    public ObservableCollection<Theme> Presets { get; }
    
    public ObservableCollection<Language> Languages { get; }
    
    public Theme SelectedTheme
    {
        get => _selectedTheme;
        set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
    }

    public Language SelectedLanguage
    {
        get => _selectedLanguage;
        set => this.RaiseAndSetIfChanged(ref _selectedLanguage, value);
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
    
    public string VisibleImage
    {
        get => _visibleImage;
        set => this.RaiseAndSetIfChanged(ref _visibleImage, value);
    }
    
    public string LoadFile
    {
        get => _loadFIle;
        set => this.RaiseAndSetIfChanged(ref _loadFIle, value);
    }
    
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

    public string CoverImg
    {
        get => _coverImg;
        set => this.RaiseAndSetIfChanged(ref _coverImg, value);
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
    
    public ICommand LoadFileCommand { get; }
    
    public ICommand PlayPauseCommand { get; }
    
    public ICommand VolumeCommand { get; }
    
    public ICommand BackTime { get; }
    
    public ICommand ForeTime { get; }
    
    public ICommand OpenEqualizerMenuCommand { get; }

    public string? UrlPathSegment => "/main";
    
    public IScreen HostScreen { get; }
    
    public MainPageViewModel()
    {
        Presets = new ObservableCollection<Theme>
        {
            new Theme
            {
                Name = "Клубника", 
                PrimaryColor = "#ff595e", 
                SecondaryColor = "#ffdadc", 
                ThirdColor = "#ffeded",
                FourthColor = "#ffacae",
                HomeButton = "Assets/HomeRed.png", 
                MusicButton = "Assets/MusicCollectionRed.png", 
                VideoButton = "Assets/VideoCollectionRed.png",
                PlaylistButton = "Assets/PlaylistRed.png",
                SettingsButton = "Assets/SettingsRed.png",
                PlayButton = "Assets/FocusRed.png",
                StopButton = "Assets/StopRed.png",
                BackTenButton = "Assets/BackTimeRed.png",
                ForeTenButton = "Assets/ForeTimeRed.png",
                BackwardButton = "Assets/BackwardRed.png",
                ForewardButton = "Assets/ForwardRed.png",
                EqualizerButton = "Assets/EqualizerRed.png",
                EditAudioButton = "Assets/EditAudioRed.png",
                EditVideoButton = "Assets/EditVideoRed.png",
                VolumeOnButton = "Assets/VolumeOnRed.png",
                VolumeOffButton = "Assets/VolumeOffRed.png",
                ListStopBack = "Assets/ListStopRed.png",
                MainBack = "Assets/MainPagePictureRed2.png",
                MusicBack = "Assets/MusicPagePictureRed2.png",
                VideoBack = "Assets/VideoPagePictureRed2.png",
                PlaylistBack = "Assets/PlaylistPagePictureRed2.png",
                Cover = "Assets/strawberry.jpg",
                Close = "Assets/CloseRed.png",
                Roll = "Assets/RollRed.png",
                Maximise = "Assets/MaximiseRed.png",
                Minimize = "Assets/MinimizeRed.png"
            },
            new Theme
            {
                Name = "Лимонный шербет", 
                PrimaryColor = "#ffca3a", 
                SecondaryColor = "#fff3d4",
                ThirdColor = "#fff9e9",
                FourthColor = "#ffe49c",
                HomeButton = "Assets/HomeYello.png", 
                MusicButton = "Assets/MusicCollectionYello.png", 
                VideoButton = "Assets/VideoCollectionYello.png",
                PlaylistButton = "Assets/PlaylistYello.png",
                SettingsButton = "Assets/SettingsYello.png",
                PlayButton = "Assets/FocusYellow.png",
                StopButton = "Assets/StopYellow.png",
                BackTenButton = "Assets/BackTimeYellow.png",
                ForeTenButton = "Assets/ForeTimeYellow.png",
                BackwardButton = "Assets/BackwardYellow.png",
                ForewardButton = "Assets/ForwardYellow.png",
                EqualizerButton = "Assets/EqualizerYellow.png",
                EditAudioButton = "Assets/EditAudioYellow.png",
                EditVideoButton = "Assets/EditVideoYellow.png",
                VolumeOnButton = "Assets/VolumeOnYellow.png",
                VolumeOffButton = "Assets/VolumeOffYellow.png",
                ListStopBack = "Assets/ListStopYellow.png",
                MainBack = "Assets/MainPagePictureYellow2.png",
                MusicBack = "Assets/MusicPagePictureYellow2.png",
                VideoBack = "Assets/VideoPagePictureYellow2.png",
                PlaylistBack = "Assets/PlaylistPagePictureYellow2.png",
                Cover = "Assets/lemon sherbet.jpg",
                Close = "Assets/CloseYellow.png",
                Roll = "Assets/RollYellow.png",
                Maximise = "Assets/MaximiseYellow.png",
                Minimize = "Assets/MinimizeYellow.png"
            },
            new Theme
            {
                Name = "Фисташка", 
                PrimaryColor = "#8ac926", 
                SecondaryColor = "#e5f3cf", 
                ThirdColor = "#f2f9e7",
                FourthColor = "#c4e492",
                HomeButton = "Assets/HomeGreen.png", 
                MusicButton = "Assets/MusicCollectionGreen.png", 
                VideoButton = "Assets/VideoCollectionGreen.png",
                PlaylistButton = "Assets/PlaylistGreen.png",
                SettingsButton = "Assets/SettingsGreen.png",
                PlayButton = "Assets/FocusGreen.png",
                StopButton = "Assets/StopGreen.png",
                BackTenButton = "Assets/BackTimeGreen.png",
                ForeTenButton = "Assets/ForeTimeGreen.png",
                BackwardButton = "Assets/BackwardGreen.png",
                ForewardButton = "Assets/ForwardGreen.png",
                EqualizerButton = "Assets/EqualizerGreen.png",
                EditAudioButton = "Assets/EditAudioGreen.png",
                EditVideoButton = "Assets/EditVideoGreen.png",
                VolumeOnButton = "Assets/VolumeOnGreen.png",
                VolumeOffButton = "Assets/VolumeOffGreen.png",
                ListStopBack = "Assets/ListStopGreen.png",
                MainBack = "Assets/MainPagePictureGreen2.png",
                MusicBack = "Assets/MusicPagePictureGreen2.png",
                VideoBack = "Assets/VideoPagePictureGreen2.png",
                PlaylistBack = "Assets/PlaylistPagePictureGreen2.png",
                Cover = "Assets/pistachios.jpg",
                Close = "Assets/CloseGreen.png",
                Roll = "Assets/RollGreen.png",
                Maximise = "Assets/MaximiseGreen.png",
                Minimize = "Assets/MinimizeGreen.png"
            },
            new Theme
            {
                Name = "Голубика", 
                PrimaryColor = "#1982c4", 
                SecondaryColor = "#cce4f2", 
                ThirdColor = "#e6f1f9",
                FourthColor = "#8cc0e1", 
                HomeButton = "Assets/HomeBlue.png", 
                MusicButton = "Assets/MusicCollectionBlue.png", 
                VideoButton = "Assets/VideoCollectionBlue.png",
                PlaylistButton = "Assets/PlaylistBlue.png",
                SettingsButton = "Assets/SettingsBlue.png",
                PlayButton = "Assets/FocusBlue.png",
                StopButton = "Assets/StopBlue.png",
                BackTenButton = "Assets/BackTimeBlue.png",
                ForeTenButton = "Assets/ForeTimeBlue.png",
                BackwardButton = "Assets/BackwardBlue.png",
                ForewardButton = "Assets/ForwardBlue.png",
                EqualizerButton = "Assets/EqualizerBlue.png",
                EditAudioButton = "Assets/EditAudioBlue.png",
                EditVideoButton = "Assets/EditVideoBlue.png",
                VolumeOnButton = "Assets/VolumeOnBlue.png",
                VolumeOffButton = "Assets/VolumeOffBlue.png",
                ListStopBack = "Assets/ListStopBlue.png",
                MainBack = "Assets/MainPagePictureBlue2.png",
                MusicBack = "Assets/MusicPagePictureBlue2.png",
                VideoBack = "Assets/VideoPagePictureBlue2.png",
                PlaylistBack = "Assets/PlaylistPagePictureBlue2.png",
                Cover = "Assets/blueberry.jpg",
                Close = "Assets/CloseBlue.png",
                Roll = "Assets/RollBlue.png",
                Maximise = "Assets/MaximiseBlue.png",
                Minimize = "Assets/MinimizeBlue.png"
            },
            new Theme
            {
                Name = "Ежевика", 
                PrimaryColor = "#6a4c93", 
                SecondaryColor = "#ded8e7", 
                ThirdColor = "#efebf3",
                FourthColor = "#b4a5c9", 
                HomeButton = "Assets/HomeViolet.png", 
                MusicButton = "Assets/MusicCollectionViolet.png", 
                VideoButton = "Assets/VideoCollectionViolet.png",
                PlaylistButton = "Assets/PlaylistViolet.png",
                SettingsButton = "Assets/SettingsViolet.png",
                PlayButton = "Assets/FocusViolet.png",
                StopButton = "Assets/StopViolet.png",
                BackTenButton = "Assets/BackTimeViolet.png",
                ForeTenButton = "Assets/ForeTimeViolet.png",
                BackwardButton = "Assets/BackwardViolet.png",
                ForewardButton = "Assets/ForwardViolet.png",
                EqualizerButton = "Assets/EqualizerViolet.png",
                EditAudioButton = "Assets/EditAudioViolet.png",
                EditVideoButton = "Assets/EditVideoViolet.png",
                VolumeOnButton = "Assets/VolumeOnViolet.png",
                VolumeOffButton = "Assets/VolumeOffViolet.png",
                ListStopBack = "Assets/ListStopViolet.png",
                MainBack = "Assets/MainPagePictureViolet2.png",
                MusicBack = "Assets/MusicPagePictureViolet2.png",
                VideoBack = "Assets/VideoPagePictureViolet2.png",
                PlaylistBack = "Assets/PlaylistPagePictureViolet2.png",
                Cover = "Assets/grape.jpg",
                Close = "Assets/CloseViolet.png",
                Roll = "Assets/RollViolet.png",
                Maximise = "Assets/MaximiseViolet.png",
                Minimize = "Assets/MinimizeViolet.png"
            }
        };
        
        SelectedTheme = Presets.First();
        
        // Подписка на смену темы
        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(theme => ApplyTheme(theme));
        
        var savedThemeName = LoadSelectedThemeName();
        if (!string.IsNullOrEmpty(savedThemeName))
        {
            var savedTheme = Presets.FirstOrDefault(t => t.Name == savedThemeName);
            if (savedTheme != null)
                SelectedTheme = savedTheme;
            else
                SelectedTheme = Presets.First();
        }
        else
        {
            SelectedTheme = Presets.First();
        }
        
        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(theme =>
            {
                ApplyTheme(theme);
                SaveBothSettings(theme?.Name, SelectedLanguage?.LanguageName);
            });

        Languages = new ObservableCollection<Language>
        {
            new Language
            {
                LanguageName = "Русский",
                RedThemeName = "Клубника",
                YellowThemeName = "Лимонный шербет",
                GreenThemeName = "Фисташка",
                BlueThemeName = "Голубика",
                VioletThemeName = "Ежевика",
                FindFileButton = "Найти файл",
                EditFileButton = "Редактировать",
                FirstEqualizerThemeName = "Поп",
                SecondEqualizerThemeName = "Вокал",
                ThirdEqualizerThemeName = "Рок",
                FourthEqualizerThemeName = "Джаз",
                FifthEqualizerThemeName = "Классический",
                SixthEqualizerThemeName = "Усиление низких частот"
            },
            new Language
            {
                LanguageName = "English",
                RedThemeName = "Strawberry",
                YellowThemeName = "Lemon sherbet",
                GreenThemeName = "Pistachio",
                BlueThemeName = "Blueberry",
                VioletThemeName = "Blackberry",
                FindFileButton = "Find the file",
                EditFileButton = "Edit",
                FirstEqualizerThemeName = "Pop",
                SecondEqualizerThemeName = "Vocal",
                ThirdEqualizerThemeName = "Rock",
                FourthEqualizerThemeName = "Jazz",
                FifthEqualizerThemeName = "Classical",
                SixthEqualizerThemeName = "Bass"
            }
        };
        
        SelectedLanguage = Languages.First();
        
        // Подписка на смену языка
        this.WhenAnyValue(x => x.SelectedLanguage)
            .Subscribe(language => ApplyLanguage(language));
        
        var savedLanguage = LoadSelectedLanguage();
        if (!string.IsNullOrEmpty(savedLanguage))
        {
            var savedLang = Languages.FirstOrDefault(t => t.LanguageName == savedLanguage);
            if (savedLang != null)
                SelectedLanguage = savedLang;
            else
                SelectedLanguage = Languages.First();
        }
        else
        {
            SelectedLanguage = Languages.First();
        }
        
        this.WhenAnyValue(x => x.SelectedLanguage)
            .Subscribe(language =>
            {
                ApplyLanguage(language);
                SaveBothSettings(SelectedTheme?.Name, language?.LanguageName);
            });
    }

    public MainPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        
        // Освобождение предыдущего ресурса, если он существует
        _audioFileReader?.Dispose();
        _waveOut?.Dispose();
        _waveOut?.Stop();
        
        TrackImage = new Bitmap("Assets/MusicPagePictureRed2.png");
        OpacityImage = 0.2;
        VisibleImage = "true";
        TrackImg = "False";
        CoverImg = "False";
        //VisibleAttention = "false";
        
        VolumeOn = "True";
        VolumeOff = "False";
        
        Play = "True";
        Stop = "False";
        
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
                //VisibleAttention = "true";
                await Task.Delay(2000);
                VisibleImage = "true";
                //VisibleAttention = "false";
            }
            else
            {
                if (_isPlaying)
                {
                    _waveOut?.Pause();
                    _timer.Stop();
                    _isPlaying = false;
                    //UpdateVolume();
                    //PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                    Play = "True";
                    Stop = "False";
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
                        //PlayImage = new Bitmap("Assets/StopRed.png");
                        Play = "False";
                        Stop = "True";
                    }
                }
                _waveOut.PlaybackStopped += (sender, e) =>
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        _isPlaying = false;
                        //PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                        Play = "True";
                        Stop = "False";
                        CurrentTime = TimeSpan.Zero;
                    });
                };
            }
        }, outputScheduler: RxApp.MainThreadScheduler);
        
        Presets = new ObservableCollection<Theme>
        {
            new Theme
            {
                Name = "Клубника", 
                PrimaryColor = "#ff595e", 
                SecondaryColor = "#ffdadc", 
                ThirdColor = "#ffeded",
                FourthColor = "#ffacae",
                HomeButton = "Assets/HomeRed.png", 
                MusicButton = "Assets/MusicCollectionRed.png", 
                VideoButton = "Assets/VideoCollectionRed.png",
                PlaylistButton = "Assets/PlaylistRed.png",
                SettingsButton = "Assets/SettingsRed.png",
                PlayButton = "Assets/FocusRed.png",
                StopButton = "Assets/StopRed.png",
                BackTenButton = "Assets/BackTimeRed.png",
                ForeTenButton = "Assets/ForeTimeRed.png",
                BackwardButton = "Assets/BackwardRed.png",
                ForewardButton = "Assets/ForwardRed.png",
                EqualizerButton = "Assets/EqualizerRed.png",
                EditAudioButton = "Assets/EditAudioRed.png",
                EditVideoButton = "Assets/EditVideoRed.png",
                VolumeOnButton = "Assets/VolumeOnRed.png",
                VolumeOffButton = "Assets/VolumeOffRed.png",
                ListStopBack = "Assets/ListStopRed.png",
                MainBack = "Assets/MainPagePictureRed2.png",
                MusicBack = "Assets/MusicPagePictureRed2.png",
                VideoBack = "Assets/VideoPagePictureRed2.png",
                PlaylistBack = "Assets/PlaylistPagePictureRed2.png",
                Cover = "Assets/strawberry.jpg",
                Close = "Assets/CloseRed.png",
                Roll = "Assets/RollRed.png",
                Maximise = "Assets/MaximiseRed.png",
                Minimize = "Assets/MinimizeRed.png"
            },
            new Theme
            {
                Name = "Лимонный шербет", 
                PrimaryColor = "#ffca3a", 
                SecondaryColor = "#fff3d4",
                ThirdColor = "#fff9e9",
                FourthColor = "#ffe49c",
                HomeButton = "Assets/HomeYello.png", 
                MusicButton = "Assets/MusicCollectionYello.png", 
                VideoButton = "Assets/VideoCollectionYello.png",
                PlaylistButton = "Assets/PlaylistYello.png",
                SettingsButton = "Assets/SettingsYello.png",
                PlayButton = "Assets/FocusYellow.png",
                StopButton = "Assets/StopYellow.png",
                BackTenButton = "Assets/BackTimeYellow.png",
                ForeTenButton = "Assets/ForeTimeYellow.png",
                BackwardButton = "Assets/BackwardYellow.png",
                ForewardButton = "Assets/ForwardYellow.png",
                EqualizerButton = "Assets/EqualizerYellow.png",
                EditAudioButton = "Assets/EditAudioYellow.png",
                EditVideoButton = "Assets/EditVideoYellow.png",
                VolumeOnButton = "Assets/VolumeOnYellow.png",
                VolumeOffButton = "Assets/VolumeOffYellow.png",
                ListStopBack = "Assets/ListStopYellow.png",
                MainBack = "Assets/MainPagePictureYellow2.png",
                MusicBack = "Assets/MusicPagePictureYellow2.png",
                VideoBack = "Assets/VideoPagePictureYellow2.png",
                PlaylistBack = "Assets/PlaylistPagePictureYellow2.png",
                Cover = "Assets/lemon sherbet.jpg",
                Close = "Assets/CloseYellow.png",
                Roll = "Assets/RollYellow.png",
                Maximise = "Assets/MaximiseYellow.png",
                Minimize = "Assets/MinimizeYellow.png"
            },
            new Theme
            {
                Name = "Фисташка", 
                PrimaryColor = "#8ac926", 
                SecondaryColor = "#e5f3cf", 
                ThirdColor = "#f2f9e7",
                FourthColor = "#c4e492",
                HomeButton = "Assets/HomeGreen.png", 
                MusicButton = "Assets/MusicCollectionGreen.png", 
                VideoButton = "Assets/VideoCollectionGreen.png",
                PlaylistButton = "Assets/PlaylistGreen.png",
                SettingsButton = "Assets/SettingsGreen.png",
                PlayButton = "Assets/FocusGreen.png",
                StopButton = "Assets/StopGreen.png",
                BackTenButton = "Assets/BackTimeGreen.png",
                ForeTenButton = "Assets/ForeTimeGreen.png",
                BackwardButton = "Assets/BackwardGreen.png",
                ForewardButton = "Assets/ForwardGreen.png",
                EqualizerButton = "Assets/EqualizerGreen.png",
                EditAudioButton = "Assets/EditAudioGreen.png",
                EditVideoButton = "Assets/EditVideoGreen.png",
                VolumeOnButton = "Assets/VolumeOnGreen.png",
                VolumeOffButton = "Assets/VolumeOffGreen.png",
                ListStopBack = "Assets/ListStopGreen.png",
                MainBack = "Assets/MainPagePictureGreen2.png",
                MusicBack = "Assets/MusicPagePictureGreen2.png",
                VideoBack = "Assets/VideoPagePictureGreen2.png",
                PlaylistBack = "Assets/PlaylistPagePictureGreen2.png",
                Cover = "Assets/pistachios.jpg",
                Close = "Assets/CloseGreen.png",
                Roll = "Assets/RollGreen.png",
                Maximise = "Assets/MaximiseGreen.png",
                Minimize = "Assets/MinimizeGreen.png"
            },
            new Theme
            {
                Name = "Голубика", 
                PrimaryColor = "#1982c4", 
                SecondaryColor = "#cce4f2", 
                ThirdColor = "#e6f1f9",
                FourthColor = "#8cc0e1",
                HomeButton = "Assets/HomeBlue.png", 
                MusicButton = "Assets/MusicCollectionBlue.png", 
                VideoButton = "Assets/VideoCollectionBlue.png",
                PlaylistButton = "Assets/PlaylistBlue.png",
                SettingsButton = "Assets/SettingsBlue.png",
                PlayButton = "Assets/FocusBlue.png",
                StopButton = "Assets/StopBlue.png",
                BackTenButton = "Assets/BackTimeBlue.png",
                ForeTenButton = "Assets/ForeTimeBlue.png",
                BackwardButton = "Assets/BackwardBlue.png",
                ForewardButton = "Assets/ForwardBlue.png",
                EqualizerButton = "Assets/EqualizerBlue.png",
                EditAudioButton = "Assets/EditAudioBlue.png",
                EditVideoButton = "Assets/EditVideoBlue.png",
                VolumeOnButton = "Assets/VolumeOnBlue.png",
                VolumeOffButton = "Assets/VolumeOffBlue.png",
                ListStopBack = "Assets/ListStopBlue.png",
                MainBack = "Assets/MainPagePictureBlue2.png",
                MusicBack = "Assets/MusicPagePictureBlue2.png",
                VideoBack = "Assets/VideoPagePictureBlue2.png",
                PlaylistBack = "Assets/PlaylistPagePictureBlue2.png",
                Cover = "Assets/blueberry.jpg",
                Close = "Assets/CloseBlue.png",
                Roll = "Assets/RollBlue.png",
                Maximise = "Assets/MaximiseBlue.png",
                Minimize = "Assets/MinimizeBlue.png"
            },
            new Theme
            {
                Name = "Ежевика", 
                PrimaryColor = "#6a4c93", 
                SecondaryColor = "#ded8e7", 
                ThirdColor = "#efebf3",
                FourthColor = "#b4a5c9", 
                HomeButton = "Assets/HomeViolet.png", 
                MusicButton = "Assets/MusicCollectionViolet.png", 
                VideoButton = "Assets/VideoCollectionViolet.png",
                PlaylistButton = "Assets/PlaylistViolet.png",
                SettingsButton = "Assets/SettingsViolet.png",
                PlayButton = "Assets/FocusViolet.png",
                StopButton = "Assets/StopViolet.png",
                BackTenButton = "Assets/BackTimeViolet.png",
                ForeTenButton = "Assets/ForeTimeViolet.png",
                BackwardButton = "Assets/BackwardViolet.png",
                ForewardButton = "Assets/ForwardViolet.png",
                EqualizerButton = "Assets/EqualizerViolet.png",
                EditAudioButton = "Assets/EditAudioViolet.png",
                EditVideoButton = "Assets/EditVideoViolet.png",
                VolumeOnButton = "Assets/VolumeOnViolet.png",
                VolumeOffButton = "Assets/VolumeOffViolet.png",
                ListStopBack = "Assets/ListStopViolet.png",
                MainBack = "Assets/MainPagePictureViolet2.png",
                MusicBack = "Assets/MusicPagePictureViolet2.png",
                VideoBack = "Assets/VideoPagePictureViolet2.png",
                PlaylistBack = "Assets/PlaylistPagePictureViolet2.png",
                Cover = "Assets/grape.jpg",
                Close = "Assets/CloseViolet.png",
                Roll = "Assets/RollViolet.png",
                Maximise = "Assets/MaximiseViolet.png",
                Minimize = "Assets/MinimizeViolet.png"
            }
        };
        
        SelectedTheme = Presets.First();
        
        // Подписка на смену темы
        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(theme => ApplyTheme(theme));
        
        var savedThemeName = LoadSelectedThemeName();
        if (!string.IsNullOrEmpty(savedThemeName))
        {
            var savedTheme = Presets.FirstOrDefault(t => t.Name == savedThemeName);
            if (savedTheme != null)
                SelectedTheme = savedTheme;
            else
                SelectedTheme = Presets.First();
        }
        else
        {
            SelectedTheme = Presets.First();
        }
        
        this.WhenAnyValue(x => x.SelectedTheme)
            .Subscribe(theme =>
            {
                ApplyTheme(theme);
                SaveBothSettings(theme?.Name, SelectedLanguage?.LanguageName);
            });
       

        Languages = new ObservableCollection<Language>
        {
            new Language
            {
                LanguageName = "Русский",
                RedThemeName = "Клубника",
                YellowThemeName = "Лимонный шербет",
                GreenThemeName = "Фисташка",
                BlueThemeName = "Голубика",
                VioletThemeName = "Ежевика",
                FindFileButton = "Найти файл",
                EditFileButton = "Редактировать",
                FirstEqualizerThemeName = "Поп",
                SecondEqualizerThemeName = "Вокал",
                ThirdEqualizerThemeName = "Рок",
                FourthEqualizerThemeName = "Джаз",
                FifthEqualizerThemeName = "Классический",
                SixthEqualizerThemeName = "Усиление низких частот"
            },
            new Language
            {
                LanguageName = "English",
                RedThemeName = "Strawberry",
                YellowThemeName = "Lemon sherbet",
                GreenThemeName = "Pistachio",
                BlueThemeName = "Blueberry",
                VioletThemeName = "Blackberry",
                FindFileButton = "Find the file",
                EditFileButton = "Edit",
                FirstEqualizerThemeName = "Pop",
                SecondEqualizerThemeName = "Vocal",
                ThirdEqualizerThemeName = "Rock",
                FourthEqualizerThemeName = "Jazz",
                FifthEqualizerThemeName = "Classical",
                SixthEqualizerThemeName = "Bass"
            }
        };
        
        SelectedLanguage = Languages.First();
        
        // Подписка на смену языка
        this.WhenAnyValue(x => x.SelectedLanguage)
            .Subscribe(language => ApplyLanguage(language));
        
        var savedLanguage = LoadSelectedLanguage();
        if (!string.IsNullOrEmpty(savedLanguage))
        {
            var savedLang = Languages.FirstOrDefault(t => t.LanguageName == savedLanguage);
            if (savedLang != null)
                SelectedLanguage = savedLang;
            else
                SelectedLanguage = Languages.First();
        }
        else
        {
            SelectedLanguage = Languages.First();
        }
        
        this.WhenAnyValue(x => x.SelectedLanguage)
            .Subscribe(language =>
            {
                ApplyLanguage(language);
                SaveBothSettings(SelectedTheme?.Name, language?.LanguageName);
            });
    }
    
    public void ApplyTheme(Theme theme)
    {
        // Здесь обновятся ресурсы приложения
        var app = Avalonia.Application.Current;
        if (app == null) return;

        // Обновление ресурсов
        app.Resources["PrimaryColor"] = Avalonia.Media.Color.Parse(theme.PrimaryColor);
        app.Resources["SecondaryColor"] = Avalonia.Media.Color.Parse(theme.SecondaryColor);
        app.Resources["ThirdColor"] = Avalonia.Media.Color.Parse(theme.ThirdColor);
        app.Resources["FourthColor"] = Avalonia.Media.Color.Parse(theme.FourthColor);
        app.Resources["HomeButton"] = new Avalonia.Media.Imaging.Bitmap(theme.HomeButton);
        app.Resources["MusicButton"] = new Avalonia.Media.Imaging.Bitmap(theme.MusicButton);
        app.Resources["VideoButton"] = new Avalonia.Media.Imaging.Bitmap(theme.VideoButton);
        app.Resources["PlaylistButton"] = new Avalonia.Media.Imaging.Bitmap(theme.PlaylistButton);
        app.Resources["SettingsButton"] = new Avalonia.Media.Imaging.Bitmap(theme.SettingsButton);
        app.Resources["PlayButton"] = new Avalonia.Media.Imaging.Bitmap(theme.PlayButton);
        app.Resources["StopButton"] = new Avalonia.Media.Imaging.Bitmap(theme.StopButton);
        app.Resources["BackTenButton"] = new Avalonia.Media.Imaging.Bitmap(theme.BackTenButton);
        app.Resources["ForeTenButton"] = new Avalonia.Media.Imaging.Bitmap(theme.ForeTenButton);
        app.Resources["BackwardButton"] = new Avalonia.Media.Imaging.Bitmap(theme.BackwardButton);
        app.Resources["ForewardButton"] = new Avalonia.Media.Imaging.Bitmap(theme.ForewardButton);
        app.Resources["EqualizerButton"] = new Avalonia.Media.Imaging.Bitmap(theme.EqualizerButton);
        app.Resources["EditAudioButton"] = new Avalonia.Media.Imaging.Bitmap(theme.EditAudioButton);
        app.Resources["EditVideoButton"] = new Avalonia.Media.Imaging.Bitmap(theme.EditVideoButton);
        app.Resources["VolumeOnButton"] = new Avalonia.Media.Imaging.Bitmap(theme.VolumeOnButton);
        app.Resources["VolumeOffButton"] = new Avalonia.Media.Imaging.Bitmap(theme.VolumeOffButton);
        app.Resources["ListStopBack"] = new Avalonia.Media.Imaging.Bitmap(theme.ListStopBack);
        app.Resources["MainBack"] = new Avalonia.Media.Imaging.Bitmap(theme.MainBack);
        app.Resources["MusicBack"] = new Avalonia.Media.Imaging.Bitmap(theme.MusicBack);
        app.Resources["VideoBack"] = new Avalonia.Media.Imaging.Bitmap(theme.VideoBack);
        app.Resources["PlaylistBack"] = new Avalonia.Media.Imaging.Bitmap(theme.PlaylistBack);
        app.Resources["Cover"] = new Avalonia.Media.Imaging.Bitmap(theme.Cover);
        app.Resources["Close"] = new Avalonia.Media.Imaging.Bitmap(theme.Close);
        app.Resources["Roll"] = new Avalonia.Media.Imaging.Bitmap(theme.Roll);
        app.Resources["Minimize"] = new Avalonia.Media.Imaging.Bitmap(theme.Minimize);
        app.Resources["Maximise"] = new Avalonia.Media.Imaging.Bitmap(theme.Maximise);
    }
    
    public string? LoadSelectedThemeName()
    {
        try
        {
            var path = GetSettingsFilePath();
            if (!File.Exists(path))
                return null;
            var json = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<UserSettings>(json);
            return settings?.SelectedThemeName;
        }
        catch
        {
            return null;
        }
    }
    
    public void ApplyLanguage(Language language)
    {
        // Здесь обновятся ресурсы приложения
        var app = Avalonia.Application.Current;
        if (app == null) return;

        // Обновление ресурсов
        app.Resources["LanguageName"] = language.LanguageName;
        app.Resources["RedThemeName"] = language.RedThemeName;
        app.Resources["YellowThemeName"] = language.YellowThemeName;
        app.Resources["GreenThemeName"] = language.GreenThemeName;
        app.Resources["BlueThemeName"] = language.BlueThemeName;
        app.Resources["VioletThemeName"] = language.VioletThemeName;
        app.Resources["FindFileButton"] = language.FindFileButton;
        app.Resources["EditFileButton"] = language.EditFileButton;
        app.Resources["FirstEqualizerThemeName"] = language.FirstEqualizerThemeName;
        app.Resources["SecondEqualizerThemeName"] = language.SecondEqualizerThemeName;
        app.Resources["ThirdEqualizerThemeName"] = language.ThirdEqualizerThemeName;
        app.Resources["FourthEqualizerThemeName"] = language.FourthEqualizerThemeName;
        app.Resources["FifthEqualizerThemeName"] = language.FifthEqualizerThemeName;
        app.Resources["SixthEqualizerThemeName"] = language.SixthEqualizerThemeName;
    }
    
    public string? LoadSelectedLanguage()
    {
        try
        {
            var path = GetSettingsFilePath();
            if (!File.Exists(path))
                return null;
            var json = File.ReadAllText(path);
            var settings = JsonSerializer.Deserialize<UserSettings>(json);
            return settings?.SelectedLanguageName;
        }
        catch
        {
            return null;
        }
    }
    
    public void SaveBothSettings(string themeName, string languageName)
    {
        try
        {
            var settings = new UserSettings
            {
                SelectedThemeName = themeName,
                SelectedLanguageName = languageName
            };
        
            var json = JsonSerializer.Serialize(settings);
            var path = GetSettingsFilePath();
            File.WriteAllText(path, json);
        }
        catch
        {
            // *
        }
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
            //Main = title;
            //PreMain = performer;

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
            else
            {
                CoverImg = "True";
                VisibleImage = "False";
                TrackImg = "False";
                OpacityImage = 1;
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
            //VolumeImage = new Bitmap("Assets/VolumeOffRed.png");
            VolumeOn = "False";
            VolumeOff = "True";
        }
        else
        {
            //VolumeImage = new Bitmap("Assets/VolumeOnRed.png");
            VolumeOn = "True";
            VolumeOff = "False";
        }
    }
}