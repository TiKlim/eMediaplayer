using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Mediaplayer2.ViewModels;

namespace Mediaplayer2.Views;

public partial class MusicPageView : ReactiveUserControl<MusicPageViewModel>
{
    public MusicPageView()
    {
        InitializeComponent();
        /*this.WhenActivated(disposables =>
        {
            this.OneWayBind(ViewModel, x => x.UrlPathSegment, x => x.PathTextBlock.Text)
                .DisposeWith(disposables);
        });*/
    }
}