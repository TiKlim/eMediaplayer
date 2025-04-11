using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;

namespace Mediaplayer2.Views;

public partial class MainWindow : Window
{
    // Переменная для кнопок развернуть/вернуть прежний размер.
    private bool _boobool;
    public MainWindow()
    {
        InitializeComponent();
    }
    // Метод для перемещения окна по экрану с помощью верхней панели.
    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }
    // Метод для добавления возможности изменять размеры окна.
    void SetupSide(string name, StandardCursorType cursor, WindowEdge edge)
    {
        var ctl = this.Get<Control>(name);
        ctl.Cursor = new Cursor(cursor);
        ctl.PointerPressed += (i, e) =>
        {
            BeginResizeDrag(edge, e);
        };
    }
    // Метод инициализации, в котором задаются функции управления окном.
    private void InitializeComponent()
    {
        // 
        AvaloniaXamlLoader.Load(this);
        // Кнопки управления окном (свернуть/развернуть/вернуть прежний размер/закрыть).
        this.Get<Button>("MinimizeWindow").Click += delegate { this.WindowState = WindowState.Minimized; };
        this.Get<Button>("MaximizeWindow").Click += delegate { this.WindowState = WindowState.Maximized; _boobool = true; MaxMinButton(); };
        this.Get<Button>("CloseWindow").Click += delegate { this.Close(); };
        this.Get<Button>("NormalWindow").Click += delegate { this.WindowState = WindowState.Normal; _boobool = false; MaxMinButton(); };
        // Добавление возможности изменять размеры окна.
        SetupSide("Left", StandardCursorType.LeftSide, WindowEdge.West);
        SetupSide("Right", StandardCursorType.RightSide, WindowEdge.East);
        SetupSide("Top", StandardCursorType.TopSide, WindowEdge.North);
        SetupSide("Bottom", StandardCursorType.BottomSide, WindowEdge.South);
        SetupSide("TopLeft", StandardCursorType.TopLeftCorner, WindowEdge.NorthWest);
        SetupSide("TopRight", StandardCursorType.TopRightCorner, WindowEdge.NorthEast);
        SetupSide("BottomLeft", StandardCursorType.BottomLeftCorner, WindowEdge.SouthWest);
        SetupSide("BottomRight", StandardCursorType.BottomRightCorner, WindowEdge.SouthEast);
    }
    // Метод для кнопок развернуть/вернуть прежний размер.
    private void MaxMinButton()
    {
        if (_boobool)
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