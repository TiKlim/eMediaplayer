using System.Collections.ObjectModel;
using System.Reactive;
using ReactiveUI;

namespace Mediaplayer2.ViewModels;

public class MainPageViewModel : ReactiveObject
{
    public string Main { get; } = "Главная";
    
    public string PreMain { get; } = "Чем займёмся сегодня?";
    
    public IScreen HostScreen { get; }
    
    //public MainPageViewModel(IScreen hostScreen) => HostScreen = hostScreen;
    
    
}