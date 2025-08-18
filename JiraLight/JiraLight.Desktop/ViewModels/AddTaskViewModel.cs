using Avalonia.Controls.ApplicationLifetimes;
using JiraLight.Desktop.Models;
using JiraLight.Desktop.Services;
using JiraLight.Desktop.Views;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace JiraLight.Desktop.ViewModels;

public class AddTaskViewModel : ReactiveObject
{
    private string _title;
    private string _description;
    private UserModel _selectedUser;
    public string Status { get; set; } = "ToDo";
    public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>
    {
        "ToDo", "InProgress", "Done"
    };

    public ObservableCollection<UserModel> Users { get; }
    private readonly DashboardViewModel _dashboard;

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public string Description
    {
        get => _description;
        set => this.RaiseAndSetIfChanged(ref _description, value);
    }

    public UserModel SelectedUser
    {
        get => _selectedUser;
        set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
    }

    public ReactiveCommand<Unit, TaskModel> SaveCommand { get; }

    public AddTaskViewModel(DashboardViewModel dashboard)
    {
        _dashboard = dashboard;

        // Загружаем пользователей из LocalDataService
        Users = new ObservableCollection<UserModel>(LocalDataService.LoadUsers());

        // По умолчанию назначаем текущего пользователя
        SelectedUser = LocalDataService.CurrentUser;

        SaveCommand = ReactiveCommand.Create(() =>
        {
            var task = new TaskModel
            {
                Title = Title,
                Description = Description,
                Status = Status switch
                {
                    "ToDo" => TaskStatus.ToDo,
                    "InProgress" => TaskStatus.InProgress,
                    "Done" => TaskStatus.Done,
                    _ => TaskStatus.ToDo
                },
                CreateUser = LocalDataService.CurrentUser,
                AssignedUser = SelectedUser
            };

            _dashboard.AddTask(task);

            // Показ уведомления
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var notification = new AddNotificationWindow();
                notification.Show(desktop.MainWindow);
            }

            return task;
        });
    }
}
