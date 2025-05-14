using Mediaplayer2.Models;
using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class SettingsPageViewModel : ViewModelBase, IRoutableViewModel
{
    public string Main { get; }
    
    public string PreMain { get; }
    
    public string? UrlPathSegment => "/settings";
    
    public IScreen HostScreen { get; }
    
    public Equalizer EqualizerValue { get; }

    public SettingsPageViewModel()
    {
        
    }

    public SettingsPageViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        
        Main = "Настройки";
        PreMain = "Настрой под настроение";

        EqualizerValue = new Equalizer();
    }
}