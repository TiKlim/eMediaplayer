using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Mediaplayer2.ViewModels;

namespace Mediaplayer2.Views;

public partial class EditVideoView : ReactiveUserControl<EditVideoViewModel>
{
    public EditVideoView()
    {
        InitializeComponent();
    }
}