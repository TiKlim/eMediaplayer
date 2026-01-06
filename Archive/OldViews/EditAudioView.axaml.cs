using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Mediaplayer2.ViewModels;

namespace Mediaplayer2.Views;

public partial class EditAudioView : ReactiveUserControl<EditAudioViewModel>
{
    public EditAudioView()
    {
        InitializeComponent();
    }
}