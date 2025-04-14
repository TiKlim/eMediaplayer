using System.Collections.ObjectModel;

namespace Mediaplayer2.ViewModels;

public class MainPageViewModel : ViewModelBase
{
    public string Main { get; } = "Главная";
    
    public string PreMain { get; } = "Чем займёмся сегодня?";
}