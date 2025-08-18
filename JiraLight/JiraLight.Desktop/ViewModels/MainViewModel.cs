using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using JiraLight.Desktop.Services;
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
    private bool _isLoggedIn;
    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => this.RaiseAndSetIfChanged(ref _isLoggedIn, value);
    }

    private DashboardViewModel _dashboardVm;

    public ReactiveCommand<Unit, Unit> NavigateDashboardCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAddTaskDialogCommand { get; }

    public MainViewModel()
    {
        CurrentPage = new LoginPage
        {
            DataContext = new LoginViewModel(() =>
            {
                // После успешного логина
                IsLoggedIn = true;

                // Создаём DashboardViewModel с текущим пользователем
                _dashboardVm = new DashboardViewModel(LocalDataService.CurrentUser);

                CurrentPage = _dashboardVm;
            })
        };

        NavigateDashboardCommand = ReactiveCommand.Create(() =>
        {
            if (_dashboardVm == null && LocalDataService.CurrentUser != null)
            {
                _dashboardVm = new DashboardViewModel(LocalDataService.CurrentUser);
            }
            CurrentPage = _dashboardVm;
        });

        OpenAddTaskDialogCommand = ReactiveCommand.Create(() =>
        {
            if (Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainVm = (MainViewModel)desktop.MainWindow.DataContext;

                // Используем уже существующий DashboardViewModel
                var dashboard = mainVm.CurrentPage as DashboardViewModel ?? mainVm._dashboardVm;

                // Создаём VM страницы добавления задачи
                var addTaskVm = new AddTaskViewModel(dashboard);

                // Создаём страницу и устанавливаем DataContext
                mainVm.CurrentPage = new AddTaskDialog
                {
                    DataContext = addTaskVm
                };
            }
        });
    }

    // Метод, который будет вызван после успешного логина
    private void OnLoginSuccess()
    {
        CurrentPage = _dashboardVm;
    }
}
