using JiraLight.Desktop.Models;
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
    private TaskStatus _status;

    public TaskModel Task { get; }
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

    public TaskStatus Status
    {
        get => _status;
        set => this.RaiseAndSetIfChanged(ref _status, value);
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

        SaveCommand = ReactiveCommand.Create(() =>
        {
            // Обновляем модель
            Task.Title = Title;
            Task.Description = Description;

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

        BackCommand = ReactiveCommand.Create(goBack);
    }
}
