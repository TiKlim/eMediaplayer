using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Mediaplayer2.ViewModels;
using ReactiveUI;

namespace Mediaplayer2.Views;

public partial class MainPageView : ReactiveUserControl<MainPageViewModel>
{
    public MainPageView()
    {
        InitializeComponent();
        /*this.WhenActivated(disposables => { });
        AvaloniaXamlLoader.Load(this);*/
    }
}