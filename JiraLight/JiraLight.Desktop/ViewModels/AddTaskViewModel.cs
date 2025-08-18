using JiraLight.Desktop.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;

namespace JiraLight.Desktop.ViewModels;

public class AddTaskViewModel : ReactiveObject
{
    private string _title;
    private string _description;
    public string Status { get; set; } = "ToDo";
    public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>
    {
        "ToDo", "InProgress", "Done"
    };

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

    public ReactiveCommand<Unit, TaskModel> SaveCommand { get; }

    public AddTaskViewModel(DashboardViewModel dashboard)
    {
        _dashboard = dashboard;

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
                }
            };

            _dashboard.AddTask(task);
            return task;
        });
    }
}
