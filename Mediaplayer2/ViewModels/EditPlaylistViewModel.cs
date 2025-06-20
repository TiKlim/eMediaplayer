using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Mediaplayer2.Models;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class EditPlaylistViewModel : ViewModelBase, IRoutableViewModel
{
    private string _loadFIle;
    
    private string _save;

    private string _cancel;
    
    public string Main { get; }
    
    public string PreMain { get; }
    
    public string? UrlPathSegment => "/playlists";
    
    public IScreen HostScreen { get; }
    
    private string _newPlaylistName;
    
    public string NewPlaylistName
    {
        get => _newPlaylistName;
        set
        {
            if (_newPlaylistName != value)
            {
                _newPlaylistName = value;
                //OnPropertyChanged();
                CreatePlaylistCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public RelayCommand CreatePlaylistCommand { get; }
    
    public ObservableCollection<Playlist> Playlists { get; } = new ObservableCollection<Playlist>();
    
    public ReactiveCommand<Unit, IRoutableViewModel> CancelCommand { get; }
    
    public ReactiveCommand<Unit, IRoutableViewModel> SaveCommand { get; }

    public Playlist playLists;
    
    public ICommand LoadFileCommand { get; }

    public string LoadFile
    {
        get => _loadFIle;
        set => this.RaiseAndSetIfChanged(ref _loadFIle, value);
    }
    
    public string SaveFile
    {
        get => _save;
        set => this.RaiseAndSetIfChanged(ref _save, value);
    }

    public string Cancel
    {
        get => _cancel;
        set => this.RaiseAndSetIfChanged(ref _cancel, value);
    }

    public EditPlaylistViewModel()
    {
        
    }

    public EditPlaylistViewModel(Playlist playlist, IScreen? screen = null)
    {
        if (playlist == null)
            throw new ArgumentNullException(nameof(playlist));
        
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;

        Main = "Плейлисты";
        PreMain = "Собери на своё усмотрение"; 
        LoadFile = "Добавить трек";
        
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
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
            }
        }, outputScheduler: RxApp.MainThreadScheduler);
        
        CreatePlaylistCommand = new RelayCommand(CreatePlaylist, CanCreatePlaylist);
        
        SaveCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new PlaylistPageViewModel(HostScreen)).ObserveOn(RxApp.MainThreadScheduler));
        CancelCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.Navigate.Execute(new PlaylistPageViewModel(HostScreen)).ObserveOn(RxApp.MainThreadScheduler));
        
        LoadPlaylists();
    }
    
    private void LoadPlaylists()
    {
        Playlists.Clear(); // Очистка текущего списка плейлистов
    
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string playlistsDirectory = Path.Combine(appDataPath, "Mediaplayer", "Playlists");

        if (Directory.Exists(playlistsDirectory))
        {
            var directories = Directory.GetDirectories(playlistsDirectory);
            foreach (var dir in directories)
            {
                var playlist = new Playlist
                {
                    Name = Path.GetFileName(dir),
                    FolderPath = dir
                };
                Playlists.Add(playlist);
            }
        }
    }
    
    private bool CanCreatePlaylist()
    {
        return !string.IsNullOrWhiteSpace(NewPlaylistName);
    }

    private void CreatePlaylist()
    {
        try
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string playlistsDirectory = Path.Combine(appDataPath, "Mediaplayer", "Playlists");
            Directory.CreateDirectory(playlistsDirectory);

            string playlistFolderPath = Path.Combine(playlistsDirectory, NewPlaylistName);
            if (Directory.Exists(playlistFolderPath))
            {
                // Папка с таким именем уже существует — уведомление
                return;
            }

            Directory.CreateDirectory(playlistFolderPath);

            var playlist = new Playlist
            {
                Name = NewPlaylistName,
                FolderPath = playlistFolderPath
            };
            
            playlist.Save();

            NewPlaylistName = string.Empty;
            
            LoadPlaylists();
        }
        catch (Exception ex)
        {
            // Обработка
        }
    }
}