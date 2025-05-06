using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class PlaylistPageViewModel : ViewModelBase, IRoutableViewModel
{
    public string Main { get; }
    
    public string PreMain { get; }
    
    public string? UrlPathSegment => "/playlist";
    
    public IScreen HostScreen { get; }

    public PlaylistPageViewModel()
    {
        
    }

    public PlaylistPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;

        Main = "Плейлисты";
        PreMain = "Похоже, что у Вас нет плейлистов :(";
    }
}