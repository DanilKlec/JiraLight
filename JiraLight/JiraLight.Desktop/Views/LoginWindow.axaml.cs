using Avalonia.Interactivity;
using Avalonia.ReactiveUI;
using JiraLight.Desktop.ViewModels;
using ReactiveUI;

namespace JiraLight.Desktop.Views;

public partial class LoginWindow : ReactiveWindow<LoginViewModel>
{
    public LoginWindow()
    {
        InitializeComponent();
        this.WhenActivated(d => { });
    }

    private void Minimize_Click(object sender, RoutedEventArgs e)
    {
        WindowState = Avalonia.Controls.WindowState.Minimized;
    }

    private void Maximize_Click(object sender, RoutedEventArgs e)
    {
        if (WindowState == Avalonia.Controls.WindowState.Maximized)
        {
            WindowState = Avalonia.Controls.WindowState.Normal;
        }
        else
        {
            WindowState = Avalonia.Controls.WindowState.Maximized;
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}