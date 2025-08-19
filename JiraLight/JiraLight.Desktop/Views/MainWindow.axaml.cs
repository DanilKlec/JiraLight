using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.ReactiveUI;
using JiraLight.Desktop.ViewModels;
using ReactiveUI;

namespace JiraLight.Desktop.Views;

public partial class MainWindow : ReactiveWindow<MainViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => { /* Подписки здесь */ });

        // Назначаем обработчики событий в конструкторе
        this.FindControl<Button>("MinimizeButton")?.AddHandler(Button.ClickEvent, (_, __) =>
            this.WindowState = Avalonia.Controls.WindowState.Minimized);

        this.FindControl<Button>("MaximizeButton")?.AddHandler(Button.ClickEvent, (_, __) =>
            this.WindowState = this.WindowState == Avalonia.Controls.WindowState.Maximized
                ? Avalonia.Controls.WindowState.Normal
                : Avalonia.Controls.WindowState.Maximized);

        this.FindControl<Button>("CloseButton")?.AddHandler(Button.ClickEvent, (_, __) =>
            this.Close());
    }

    private void TopBar_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }
}
