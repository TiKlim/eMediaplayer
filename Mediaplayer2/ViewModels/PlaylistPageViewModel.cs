using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using Mediaplayer2.Models;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class PlaylistPageViewModel : ViewModelBase, IRoutableViewModel
{
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
    
    public ReactiveCommand<Playlist, IRoutableViewModel> PlaylistClickedCommand { get; }

    public Playlist playLists;


    public PlaylistPageViewModel()
    {
        
    }

    public PlaylistPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;

        Main = "Плейлисты";
        PreMain = "Похоже, что у Вас нет плейлистов"; 
        
        CreatePlaylistCommand = new RelayCommand(CreatePlaylist, CanCreatePlaylist);
        
        // Создаем команду, которая возвращает IRoutableViewModel
        /*PlaylistClickedCommand = ReactiveCommand.Create<Playlist, IRoutableViewModel>(playlist =>
            new MusicFromListViewModel(playlist, HostScreen));

        // Подписываемся на выполнение команды для навигации
        PlaylistClickedCommand.Subscribe(vm =>
            HostScreen.Router.Navigate.Execute(vm));*/
        PlaylistClickedCommand = ReactiveCommand.Create<Playlist, IRoutableViewModel>(playlist =>
        {
            var viewModel = new MusicFromListViewModel(playlist, HostScreen);
            return viewModel;
        });

// Подписка на выполнение команды для навигации
        PlaylistClickedCommand.Subscribe(vm =>
        {
            HostScreen.Router.Navigate.Execute(vm);
        });


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