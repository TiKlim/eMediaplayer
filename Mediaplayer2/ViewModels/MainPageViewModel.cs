using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class MainPageViewModel : ViewModelBase, IRoutableViewModel
{
    public string Main { get; } = "Главная";
    
    public string PreMain { get; } = "Чем займёмся сегодня?";

    public string? UrlPathSegment => "/home";
    public IScreen HostScreen { get; }

    public MainPageViewModel()
    {
        HostScreen = Locator.Current.GetService<IScreen>()!;
    }
    
    public MainPageViewModel(IScreen? hostScreen = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
    }
    
    
}