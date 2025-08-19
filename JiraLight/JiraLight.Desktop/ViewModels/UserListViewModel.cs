using JiraLight.Desktop.Models;
using JiraLight.Desktop.Services;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace JiraLight.Desktop.ViewModels;
public class UserListViewModel : ReactiveObject
{
    public ObservableCollection<UserModel> AllUsers { get; }
    private ObservableCollection<UserModel> _filteredTasks;
    public ObservableCollection<UserModel> FilteredTasks
    {
        get => _filteredTasks;
        set => this.RaiseAndSetIfChanged(ref _filteredTasks, value);
    }

    private string _filterUsername;
    public string FilterUsername
    {
        get => _filterUsername;
        set => this.RaiseAndSetIfChanged(ref _filterUsername, value);
    }

    public ReactiveCommand<Unit, Unit> ApplyFiltersCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearFiltersCommand { get; }

    public UserListViewModel()
    {
        var users = LocalDataService.LoadUsers();
        FilteredTasks = new ObservableCollection<UserModel>(users);

        ApplyFiltersCommand = ReactiveCommand.Create(() =>
        {
            var filtered = users.Where(t =>
                (string.IsNullOrWhiteSpace(FilterUsername) || t.Username.Contains(FilterUsername))
            ).ToList();

            FilteredTasks.Clear();
            foreach (var t in filtered)
                FilteredTasks.Add(t);
        });

        ClearFiltersCommand = ReactiveCommand.Create(() =>
        {
            FilterUsername = string.Empty;

            FilteredTasks = new ObservableCollection<UserModel>(users);
        });

        foreach (var user in users)
        {
            user.PropertyChanged += (s, e) =>
            {
                // Здесь вызываем сохранение при каждом изменении
                LocalDataService.SaveUsers(users);
            };
        }
    }
}
