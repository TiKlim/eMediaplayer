using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
using Mediaplayer2.ViewModels;

namespace Mediaplayer2.Views;

public partial class VideoPageView : ReactiveUserControl<VideoPageViewModel>
{
    //private VideoView _videoViewl;
    
    public VideoPageView()
    {
        InitializeComponent();
        //Unloaded += OnUnloaded;
    }

    /*private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (VideoPlayer.MediaPlayer != null)
        {
            VideoPlayer.MediaPlayer = null; // Отвязать VideoView от MediaPlayer
        }
    }*/
}