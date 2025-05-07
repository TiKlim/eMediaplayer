using System;
using System.Collections.Generic;
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
    
    private System.Timers.Timer _timer;
    
    private IRoutableViewModel _routableViewModelImplementation;
    
    //private object _currentView;

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
    
    /*public object CurrentView
    {
        get => _currentView;
        set => this.RaiseAndSetIfChanged(ref _currentView, value);
    }*/
    
    public ICommand LoadFileCommand { get; }
    
    //public ReactiveCommand<Unit, IRoutableViewModel> ToHome { get; }
    
    public ICommand PlayPauseCommand { get; }
    
    public ICommand VolumeCommand { get; }
    
    public ICommand BackTime { get; }
    
    public ICommand ForeTime { get; }
    
    //public ReactiveCommand<Unit, Unit> ToAudioEditPageCommand { get; }

    public string? UrlPathSegment => "/music";
    public IScreen HostScreen { get; }
    
    public ReactiveCommand<Unit, IRoutableViewModel> ToEditAudioPageCommand { get; }

    public MusicPageViewModel()
    {

    }

    public MusicPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        Main = "Аудиоплеер";
        PreMain = "Что послушаем сегодня?";
        TrackImage = new Bitmap("Assets/MusicPagePictureRed.png");
        OpacityImage = 0.2;
        //CurrentView = new MusicPageView();
        
        _timer = new System.Timers.Timer(100); // Обновление каждые 100 мс
        _timer.Elapsed += (sender, e) =>
        {
            if (_audioFileReader != null && _isPlaying)
            {
                CurrentTime = _audioFileReader.CurrentTime;
            }
        };
        
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
                
                // Освобождение предыдущего ресурса, если он существует
                _audioFileReader?.Dispose();
                _waveOut?.Dispose();
                
                _audioFileReader = new AudioFileReader(_filePath);
                
                //_audioDuration = _audioFileReader.TotalTime; 
                //Value = _audioDuration.TotalSeconds;
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_audioFileReader); 
                AudioDuration = _audioFileReader.TotalTime;
            }
        }, outputScheduler: RxApp.MainThreadScheduler);

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
        
        ToEditAudioPageCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new EditAudioViewModel(_filePath, HostScreen)).ObserveOn(RxApp.MainThreadScheduler));
        
        /*_waveOut.PlaybackStopped += (sender, e) =>
        {
            //Dispatcher.UIThread.InvokeAsync(() =>
            //{
                _isPlaying = false;
                PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                CurrentTime = TimeSpan.Zero;
           // });
        };*/
        
        //ToAudioEditPageCommand = ReactiveCommand.Create(AudioEditPage);
    }
    
    /*private void AudioEditPage()
    {
        CurrentView = new EditAudioView();
    }*/
    
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
            AudioDuration = _totalTime;
        }
    }

    /*private async void PlayPause()
    {
        _isPlaying = true;
        await PlayAudioAsync(_filePath);
        //_audioFileReader.CurrentTime = TimeSpan.FromSeconds(StartSlider.Value);
        _isPlaying = true;
        await PlayAudioAsync(_filePath);
    }*/

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
    
    private async Task PlayAudioAsync(string filePath) 
    { 
        using (var audioFileReader = new AudioFileReader(filePath)) 
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
                //Dispatcher.UIThread.InvokeAsync(() => { CurrentTime = audioFileReader.CurrentTime; });
                CurrentTime = audioFileReader.CurrentTime;
                //await Task.Delay(10); 
            }

            if (waveOut.PlaybackState == PlaybackState.Stopped || waveOut.PlaybackState == PlaybackState.Paused)
            {
                //Dispatcher.UIThread.InvokeAsync(() =>
                //{
                    _isPlaying = false;
                    PlayImage = new Bitmap("Assets/ButtonPlayRed.png");
                //});
            }
        }
    }
}