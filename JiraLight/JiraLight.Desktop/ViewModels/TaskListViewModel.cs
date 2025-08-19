using JiraLight.Desktop.Models;
using JiraLight.Desktop.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace JiraLight.Desktop.ViewModels;

public class TaskListViewModel : ReactiveObject
{
    public ObservableCollection<TaskModel> AllTasks { get; }
    public ObservableCollection<TaskStatus?> TaskStatusList { get; }
    private ObservableCollection<TaskModel> _filteredTasks;
    public ObservableCollection<TaskModel> FilteredTasks
    {
        get => _filteredTasks;
        set => this.RaiseAndSetIfChanged(ref _filteredTasks, value);
    }

    private string _filterTitle;
    public string FilterTitle
    {
        get => _filterTitle;
        set => this.RaiseAndSetIfChanged(ref _filterTitle, value);
    }

    private UserModel _filterUser;
    public UserModel FilterUser
    {
        get => _filterUser;
        set => this.RaiseAndSetIfChanged(ref _filterUser, value);
    }

    private TaskStatus? _filterStatus;
    public TaskStatus? FilterStatus
    {
        get => _filterStatus;
        set => this.RaiseAndSetIfChanged(ref _filterStatus, value);
    }

    public ObservableCollection<UserModel> Users { get; }

    public ReactiveCommand<Unit, Unit> ApplyFiltersCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearFiltersCommand { get; }

    public TaskListViewModel()
    {
        var board = LocalDataService.LoadTasks();
        AllTasks = new ObservableCollection<TaskModel>(
            board.ToDoTasks
            .Concat(board.InProgressTasks)
            .Concat(board.DoneTasks)
        );
        TaskStatusList = new ObservableCollection<TaskStatus?>(Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus?>());
        FilteredTasks = new ObservableCollection<TaskModel>(AllTasks);

        Users = LocalDataService.LoadUsers();

        ApplyFiltersCommand = ReactiveCommand.Create(() =>
        {
            var filtered = AllTasks.Where(t =>
                (string.IsNullOrWhiteSpace(FilterTitle) || t.Title.Contains(FilterTitle)) &&
                (FilterUser == null || t.AssignedUser?.Id == FilterUser.Id) &&
                (!FilterStatus.HasValue || t.Status == FilterStatus.Value)
            ).ToList();

            FilteredTasks.Clear();
            foreach (var t in filtered)
                FilteredTasks.Add(t);
        });

        ClearFiltersCommand = ReactiveCommand.Create(() =>
        {
            FilterTitle = string.Empty;
            FilterUser = null;
            FilterStatus = null;

            FilteredTasks = new ObservableCollection<TaskModel>(AllTasks);
        });
    }
}
