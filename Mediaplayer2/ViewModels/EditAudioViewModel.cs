using ReactiveUI;
using Splat;

namespace Mediaplayer2.ViewModels;

public class EditAudioViewModel : ViewModelBase, IRoutableViewModel
{
    private string _main;

    private string _premain;
    
    public string Main
    {
        get => _main;
        set => this.RaiseAndSetIfChanged(ref _main, value);
    }

    public string PreMain
    {
        get => _premain;
        set => this.RaiseAndSetIfChanged(ref _premain, value);
    }
    
    public string? UrlPathSegment => "/editAudio";
    
    public IScreen HostScreen { get; }
    
    public EditAudioViewModel()
    {

    }

    public EditAudioViewModel(string filePath, IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        Main = "Редактор";
        
        var file = TagLib.File.Create(filePath);
        
        string title = file.Tag.Title ?? "Нет названия";
        
        string performer = file.Tag.Performers.Length > 0 ? file.Tag.Performers[0] : "Нет исполнителя";
        
        PreMain = $"{title} | {performer}";
    }
}