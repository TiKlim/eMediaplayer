using System;
using System.IO;
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

    public PlaylistPageViewModel()
    {
        
    }

    public PlaylistPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;

        Main = "Плейлисты";
        PreMain = "Похоже, что у Вас нет плейлистов"; 
        
        CreatePlaylistCommand = new RelayCommand(CreatePlaylist, CanCreatePlaylist);
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

            string filePath = Path.Combine(playlistsDirectory, $"{NewPlaylistName}.txt");
            if (File.Exists(filePath))
            {
                // Можно показать сообщение, что файл уже есть
                // В MVVM обычно через сервисы уведомлений
                return;
            }

            var playlist = new Playlist
            {
                Name = NewPlaylistName,
                FilePath = filePath
            };
            playlist.Save();

            // Очистить поле после создания
            NewPlaylistName = string.Empty;

            // Можно добавить логику обновления списка плейлистов, если есть
        }
        catch (Exception ex)
        {
            // !
        }
    }
}