using ReactiveUI;

namespace Mediaplayer2.ViewModels;

public class EditAudioViewModel : ViewModelBase
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

    public EditAudioViewModel()
    {
        Main = "Информация о файле";
        PreMain = "Отредактируй под себя";
    }
}