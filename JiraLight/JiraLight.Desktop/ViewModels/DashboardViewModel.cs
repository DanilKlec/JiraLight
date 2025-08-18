using DynamicData;
using JiraLight.Desktop.Models;
using JiraLight.Desktop.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace JiraLight.Desktop.ViewModels;

public class DashboardViewModel : ReactiveObject
{
    public ObservableCollection<TaskModel> ToDoTasks { get; }
    public ObservableCollection<TaskModel> InProgressTasks { get; }
    public ObservableCollection<TaskModel> DoneTasks { get; }

    public DashboardViewModel(UserModel currentUser)
    {
        ToDoTasks = new ObservableCollection<TaskModel>();
        InProgressTasks = new ObservableCollection<TaskModel>();
        DoneTasks = new ObservableCollection<TaskModel>();

        var board = LocalDataService.LoadTasks();

        if (board != null)
        {
            foreach (var t in board.ToDoTasks.Where(_ => _.CreateUser?.Id == currentUser?.Id || _.AssignedUser?.Id == currentUser?.Id) ?? Enumerable.Empty<TaskModel>())
                ToDoTasks.Add(t);

            foreach (var t in board.InProgressTasks.Where(_ => _.CreateUser?.Id == currentUser?.Id || _.AssignedUser?.Id == currentUser?.Id) ?? Enumerable.Empty<TaskModel>())
                InProgressTasks.Add(t);

            foreach (var t in board.DoneTasks.Where(_ => _.CreateUser?.Id == currentUser?.Id || _.AssignedUser?.Id == currentUser?.Id) ?? Enumerable.Empty<TaskModel>())
                DoneTasks.Add(t);
        }
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
