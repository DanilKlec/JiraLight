using Avalonia.Controls.ApplicationLifetimes;
using DynamicData;
using JiraLight.Desktop.Models;
using JiraLight.Desktop.Services;
using JiraLight.Desktop.Views;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace JiraLight.Desktop.ViewModels;

public class DashboardViewModel : ReactiveObject
{
    public ObservableCollection<TaskModel> ToDoTasks { get; }
    public ObservableCollection<TaskModel> InProgressTasks { get; }
    public ObservableCollection<TaskModel> DoneTasks { get; }

    private string _userName;
    public string UserName
    {
        get => _userName;
        set => this.RaiseAndSetIfChanged(ref _userName, value);
    }

    private string _initials;
    public string Initials
    {
        get => _initials;
        set => this.RaiseAndSetIfChanged(ref _initials, value);
    }

    public int ToDoCount => ToDoTasks.Count;
    public int InProgressCount => InProgressTasks.Count;
    public int DoneCount => DoneTasks.Count;

    public ReactiveCommand<Unit, Unit> LogoutCommand { get; }

    public DashboardViewModel(UserModel currentUser, MainViewModel mainVm)
    {
        ToDoTasks = new ObservableCollection<TaskModel>();
        InProgressTasks = new ObservableCollection<TaskModel>();
        DoneTasks = new ObservableCollection<TaskModel>();

        UserName = currentUser?.Name ?? "Неизвестный";
        Initials = string.Join("", UserName.Split(' ')
                                           .Where(w => !string.IsNullOrWhiteSpace(w))
                                           .Select(w => w[0]))
                                           .ToUpper();

        var board = LocalDataService.LoadTasks();

        if (board != null)
        {
            foreach (var t in board.ToDoTasks.Where(_ => _.CreateUser?.Id == currentUser?.Id || _.AssignedUser?.Id == currentUser?.Id || currentUser?.IsAdmin == true) ?? Enumerable.Empty<TaskModel>())
                ToDoTasks.Add(t);

            foreach (var t in board.InProgressTasks.Where(_ => _.CreateUser?.Id == currentUser?.Id || _.AssignedUser?.Id == currentUser?.Id || currentUser?.IsAdmin == true) ?? Enumerable.Empty<TaskModel>())
                InProgressTasks.Add(t);

            foreach (var t in board.DoneTasks.Where(_ => _.CreateUser?.Id == currentUser?.Id || _.AssignedUser?.Id == currentUser?.Id || currentUser?.IsAdmin == true) ?? Enumerable.Empty<TaskModel>())
                DoneTasks.Add(t);
        }

        LogoutCommand = ReactiveCommand.Create(() =>
        {
            mainVm.Logout();
        });

        // следим за изменениями для счетчиков
        ToDoTasks.CollectionChanged += (_, __) => this.RaisePropertyChanged(nameof(ToDoCount));
        InProgressTasks.CollectionChanged += (_, __) => this.RaisePropertyChanged(nameof(InProgressCount));
        DoneTasks.CollectionChanged += (_, __) => this.RaisePropertyChanged(nameof(DoneCount));
    }

    public void AddTask(TaskModel task)
    {
        switch (task.Status)
        {
            case TaskStatus.ToDo: ToDoTasks.Add(task); break;
            case TaskStatus.InProgress: InProgressTasks.Add(task); break;
            case TaskStatus.Done: DoneTasks.Add(task); break;
        }
        SaveAll();
    }

    public void MoveTask(TaskModel task, ObservableCollection<TaskModel> source, ObservableCollection<TaskModel> target, TaskStatus newStatus)
    {
        if (source.Remove(task))
        {
            task.Status = newStatus;
            target.Add(task);
            SaveAll();
        }
    }

    public void SaveAll()
    {
        // Сохраняем именно те коллекции, которые UI использует напрямую
        var board = new BoardState
        {
            ToDoTasks = ToDoTasks,
            InProgressTasks = InProgressTasks,
            DoneTasks = DoneTasks
        };
        LocalDataService.SaveTasks(board);
    }
}
