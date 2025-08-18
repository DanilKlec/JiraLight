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
    }
}
