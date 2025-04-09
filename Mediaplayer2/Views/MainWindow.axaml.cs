using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Mediaplayer2.Views;

public partial class MainWindow : Window
{
    private bool _boobool;
    public MainWindow()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
        this.Get<Button>("MinimizeWindow").Click += delegate { this.WindowState = WindowState.Minimized; };
        this.Get<Button>("MaximizeWindow").Click += delegate { this.WindowState = WindowState.Maximized; _boobool = true; MaxMinButton(); };
        this.Get<Button>("CloseWindow").Click += delegate { this.Close(); };
        this.Get<Button>("NormalWindow").Click += delegate { this.WindowState = WindowState.Normal; _boobool = false; MaxMinButton(); };

        
    }

    private void MaxMinButton()
    {
        if (_boobool) //???
        {
            this.Get<Button>("MaximizeWindow").IsVisible = false;
            this.Get<Button>("NormalWindow").IsVisible = true;
        }
        else
        {
            this.Get<Button>("NormalWindow").IsVisible = false;
            this.Get<Button>("MaximizeWindow").IsVisible = true;
        }
    }
}