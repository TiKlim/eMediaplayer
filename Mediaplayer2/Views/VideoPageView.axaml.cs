using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
using Mediaplayer2.ViewModels;

namespace Mediaplayer2.Views;

public partial class VideoPageView : ReactiveUserControl<VideoPageViewModel>
{
    //private VideoPageViewModel _viewModel;
    
    public VideoPageView()
    {
        InitializeComponent();
        /*_viewModel = new VideoPageViewModel();
        _viewModel.OnPlay += ViewModelOnOnPlay;
        DataContext = _viewModel;*/
        
        //videoView 
    }

    /*private void ViewModelOnOnPlay(MediaPlayer mediaPlayer)
    {
        VideoView.MediaPlayer = mediaPlayer;
    }*/
}