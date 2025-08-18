using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using JiraLight.Desktop.Views;
using ReactiveUI;
using System.Reactive;
using System.Threading.Tasks;

namespace JiraLight.Desktop.ViewModels;

public class MainViewModel : ReactiveObject
{
    private object _currentPage;
    public object CurrentPage
    {
        get => _currentPage;
        set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }

    private readonly DashboardViewModel _dashboardVm;

    public ReactiveCommand<Unit, Unit> NavigateDashboardCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAddTaskDialogCommand { get; }

    public MainViewModel()
    {
        _dashboardVm = new DashboardViewModel();
        CurrentPage = _dashboardVm;

        NavigateDashboardCommand = ReactiveCommand.Create(() =>
        {
            CurrentPage = _dashboardVm; // ������ ���� ���������
        });

        OpenAddTaskDialogCommand = ReactiveCommand.Create(() =>
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainVm = (MainViewModel)desktop.MainWindow.DataContext;

                // ���������� ��� ������������ DashboardViewModel
                var dashboard = mainVm.CurrentPage as DashboardViewModel ?? mainVm._dashboardVm;

                // ������ VM �������� ���������� ������
                var addTaskVm = new AddTaskViewModel(dashboard);

                // ������ �������� � ������������� DataContext
                mainVm.CurrentPage = new AddTaskDialog
                {
                    DataContext = addTaskVm
                };
            }
        });
    }
}
