using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using LibVLCSharp.Avalonia;
using LibVLCSharp.Shared;
using Mediaplayer2.ViewModels;

namespace Mediaplayer2.Views;

public partial class VideoShowing : ReactiveUserControl<VideoPageViewModel>
{
    public VideoShowing()
    {
        InitializeComponent();
    }
}