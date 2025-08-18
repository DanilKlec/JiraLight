using Avalonia.Controls.ApplicationLifetimes;
using JiraLight.Desktop.Models;
using JiraLight.Desktop.Services;
using JiraLight.Desktop.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using TaskStatus = JiraLight.Desktop.Models.TaskStatus;

namespace JiraLight.Desktop.ViewModels;

public class TaskDetailViewModel : ReactiveObject
{
    private string _title;
    private string _description;
    private UserModel _selectedUser;
    private TaskStatus _status;

    public TaskModel Task { get; }
    private readonly DashboardViewModel _dashboard;
    public ObservableCollection<UserModel> Users { get; }

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

    public TaskStatus Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
    }

    public UserModel SelectedUser
    {
        get => _selectedUser;
        set => this.RaiseAndSetIfChanged(ref _selectedUser, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }
    public ReactiveCommand<Unit, Unit> BackCommand { get; }

    public ObservableCollection<TaskStatus> StatusOptions { get; } =
    new ObservableCollection<TaskStatus>((TaskStatus[])Enum.GetValues(typeof(TaskStatus)));

    public TaskDetailViewModel(TaskModel task, DashboardViewModel dashboard, Action goBack)
    {
        Task = task;
        _dashboard = dashboard;

        Title = task.Title;
        Description = task.Description;
        Status = task.Status;

        // Загружаем пользователей из LocalDataService
        Users = new ObservableCollection<UserModel>(LocalDataService.LoadUsers());

        // По умолчанию назначаем текущего пользователя
        SelectedUser = task.AssignedUser;

        SaveCommand = ReactiveCommand.Create(() =>
        {
            // Обновляем модель
            Task.Title = Title;
            Task.Description = Description;
            Task.CreateUser = LocalDataService.CurrentUser;
            Task.AssignedUser = SelectedUser;
            // Если статус изменился — перемещаем задачу в другую колонку
            if (Task.Status != Status)
            {
                ObservableCollection<TaskModel> source = Task.Status switch
                {
                    TaskStatus.ToDo => _dashboard.ToDoTasks,
                    TaskStatus.InProgress => _dashboard.InProgressTasks,
                    TaskStatus.Done => _dashboard.DoneTasks,
                    _ => _dashboard.ToDoTasks
                };

                ObservableCollection<TaskModel> target = Status switch
                {
                    TaskStatus.ToDo => _dashboard.ToDoTasks,
                    TaskStatus.InProgress => _dashboard.InProgressTasks,
                    TaskStatus.Done => _dashboard.DoneTasks,
                    _ => _dashboard.ToDoTasks
                };

                _dashboard.MoveTask(Task, source, target, Status);
            }
        });

        DeleteCommand = ReactiveCommand.Create(() =>
        {
            var source = Task.Status switch
            {
                TaskStatus.ToDo => _dashboard.ToDoTasks,
                TaskStatus.InProgress => _dashboard.InProgressTasks,
                TaskStatus.Done => _dashboard.DoneTasks,
                _ => _dashboard.ToDoTasks
            };

            source.Remove(Task);
            _dashboard.SaveAll();

            // Показ уведомления
            if (Avalonia.Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var notification = new DeleteNotificationWindow();
                notification.Show(desktop.MainWindow);
            }

            goBack();
        });

        BackCommand = ReactiveCommand.Create(goBack);
    }

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
}
