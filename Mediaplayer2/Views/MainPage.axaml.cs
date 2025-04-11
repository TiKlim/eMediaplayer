using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Mediaplayer2.Views;

public partial class MainPage : UserControl
{
    public MainPage()
    {
        InitializeComponent();
    }
    
    private void MusicBtn_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        NoteMusicImage.IsVisible = true;
    }


    private void MusicBtn_OnPointerExited(object? sender, PointerEventArgs e)
    {
        NoteMusicImage.IsVisible = false;
    }

    private void VideoBtn_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        NoteVideoImage.IsVisible = true;
    }

    private void VideoBtn_OnPointerExited(object? sender, PointerEventArgs e)
    {
        NoteVideoImage.IsVisible = false;
    }

    private void PlaylistBtn_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        NotePlaylistImage.IsVisible = true;
    }

    private void PlaylistBtn_OnPointerExited(object? sender, PointerEventArgs e)
    {
        NotePlaylistImage.IsVisible = false;
    }

    private void SettingsBtn_OnPointerEntered(object? sender, PointerEventArgs e)
    {
        NoteSettingsImage.IsVisible = true;
    }

    private void SettingsBtn_OnPointerExited(object? sender, PointerEventArgs e)
    {
        NoteSettingsImage.IsVisible = false;
    }
}