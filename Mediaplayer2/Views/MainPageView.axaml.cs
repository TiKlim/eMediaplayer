using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Mediaplayer2.ViewModels;

namespace Mediaplayer2.Views;

public partial class MainPageView : ReactiveUserControl<MainPageViewModel>
{
    // Copyright (c) 2026 Timofeev Klim (eMediaplayer)
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    
    public MainPageView()
    {
        InitializeComponent();
    }
    

}