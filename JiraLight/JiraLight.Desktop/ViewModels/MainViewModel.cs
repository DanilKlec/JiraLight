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

    private bool _isAdmin;
    public bool IsAdmin
    {
        get => _isAdmin;
        set => this.RaiseAndSetIfChanged(ref _isAdmin, value);
    }

    private DashboardViewModel _dashboardVm;

    public ReactiveCommand<Unit, Unit> NavigateDashboardCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenAddTaskDialogCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenTaskListCommand { get; }
    public ReactiveCommand<Unit, Unit> OpenUserListCommand { get; }

    public MainViewModel()
    {
        CurrentPage = new LoginPage
        {
            DataContext = new LoginViewModel(() =>
            {
                // ����� ��������� ������
                IsLoggedIn = true;
                IsAdmin = LocalDataService.CurrentUser.IsAdmin == true ? true : false;
                // ������ DashboardViewModel � ������� �������������
                _dashboardVm = new DashboardViewModel(LocalDataService.CurrentUser, this);

                CurrentPage = _dashboardVm;
            }, this)
        };

        NavigateDashboardCommand = ReactiveCommand.Create(() =>
        {
            if (_dashboardVm == null && LocalDataService.CurrentUser != null)
            {
                _dashboardVm = new DashboardViewModel(LocalDataService.CurrentUser, this);
            }
            CurrentPage = _dashboardVm;
        });

        OpenTaskListCommand = ReactiveCommand.Create(() =>
        {
            CurrentPage = new TaskListPage
            {
                DataContext = new TaskListViewModel()
            };
        });

        OpenUserListCommand = ReactiveCommand.Create(() =>
        {
            CurrentPage = new UserListPage
            {
                DataContext = new UserListViewModel()
            };
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

    // �����, ������� ����� ������ ����� ��������� ������
    private void OnLoginSuccess()
    {
        IsLoggedIn = true;
        _dashboardVm = new DashboardViewModel(LocalDataService.CurrentUser, this); // ����������� ��� ������ ������������
        IsAdmin = LocalDataService.CurrentUser.IsAdmin == true ? true : false;
        CurrentPage = _dashboardVm;
    }

    public void Logout()
    {
        LocalDataService.Logout();
        IsLoggedIn = false;
        IsAdmin = false;

        // ���������� �� LoginPage
        CurrentPage = new LoginPage
        {
            DataContext = new LoginViewModel(OnLoginSuccess, this)
        };
    }
}
