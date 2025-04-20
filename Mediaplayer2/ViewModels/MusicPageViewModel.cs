using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Mediaplayer2.Views;
using NAudio.Wave;
using ReactiveUI;

namespace Mediaplayer2.ViewModels;

public class MusicPageViewModel : ReactiveObject, IRoutableViewModel
{
    public string Main { get; set; } = "Аудиоплеер";
    
    public string PreMain { get; set; } = "Что послушаем сегодня?";

    public double Value { get; set; }
    
    private string _filePath;
    private IWavePlayer _waveOut;
    private AudioFileReader _audioFileReader;
    private TimeSpan _totalTime;
    private TimeSpan _audioDuration;
    private bool _isPlaying = false;
    
    public ICommand LoadFileCommand { get; }
    
    public RoutingState Router { get; } = new RoutingState();
    
    public ReactiveCommand<Unit, IRoutableViewModel> ToHome { get; }

    public MusicPageViewModel()
    {
        LoadFileCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            /*var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "MP3 Files", Extensions = { "mp3" } }
                }
            };*/
            /*var desktop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            var mainWindow = new MainWindow();
            mainWindow.Show();
            desktop.MainWindow!.Close();
            desktop.MainWindow = mainWindow;*/
            var desctop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Filters = new List<FileDialogFilter>
                {
                    new FileDialogFilter { Name = "MP3 Files", Extensions = { "mp3" } }
                }
            };
            
            
            //var desctop = (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
            //var result = await dialog.ShowAsync(desctop);
            var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Open Text File",
                AllowMultiple = false
            });
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
        
        ToHome = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new MainPageViewModel(this))
        );
    }
    
    private void LoadMp3Info(string filePath)
    {
        var file = TagLib.File.Create(filePath);
        
        string title = file.Tag.Title ?? "Нет названия";
        
        string performer = file.Tag.Performers.Length > 0 ? file.Tag.Performers[0] : "Нет исполнителя";

        Main = $"{title}";
        PreMain = $"{performer}";
        
        if (_audioFileReader != null)
        {
            _totalTime = _audioFileReader.TotalTime;
        }
    }
}