using System;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Splat;
using Mediaplayer2.ViewModels;
using Mediaplayer2.Views;


namespace Mediaplayer2;

public static class Helper
{
    public static void Services()
    {

        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        
        Locator.CurrentMutable.Register<IViewFor<MusicPageViewModel>>(() => new MusicPageView());
    }
}