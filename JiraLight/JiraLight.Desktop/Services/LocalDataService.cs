using JiraLight.Desktop.Models;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text.Json;

namespace JiraLight.Desktop.Services
{
    internal static class LocalDataService
    {
        private static readonly string TasksFile = "tasks.json";
        private static readonly string UsersFile = "users.json";

        public static BoardState LoadTasks()
        {
            if (!File.Exists(TasksFile))
                return new BoardState
                {
                    ToDoTasks = new ObservableCollection<TaskModel>(),
                    InProgressTasks = new ObservableCollection<TaskModel>(),
                    DoneTasks = new ObservableCollection<TaskModel>()
                };

            var json = File.ReadAllText(TasksFile);

            // Попытка десериализации старого формата (список всех задач)
            try
            {
                var oldTasks = JsonSerializer.Deserialize<ObservableCollection<TaskModel>>(json);
                if (oldTasks != null)
                {
                    return new BoardState
                    {
                        ToDoTasks = new ObservableCollection<TaskModel>(oldTasks.Where(t => t.Status == TaskStatus.ToDo)),
                        InProgressTasks = new ObservableCollection<TaskModel>(oldTasks.Where(t => t.Status == TaskStatus.InProgress)),
                        DoneTasks = new ObservableCollection<TaskModel>(oldTasks.Where(t => t.Status == TaskStatus.Done))
                    };
                }
            }
            catch { }

            // Стандартная десериализация BoardState
            return JsonSerializer.Deserialize<BoardState>(json) ?? new BoardState
            {
                ToDoTasks = new ObservableCollection<TaskModel>(),
                InProgressTasks = new ObservableCollection<TaskModel>(),
                DoneTasks = new ObservableCollection<TaskModel>()
            };
        }

        public static void SaveTasks(BoardState board)
        {
            var json = JsonSerializer.Serialize(board, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TasksFile, json);
        }

        public static ObservableCollection<UserModel> LoadUsers()
        {
            if (!File.Exists(UsersFile)) return new ObservableCollection<UserModel>();
            var json = File.ReadAllText(UsersFile);
            return JsonSerializer.Deserialize<ObservableCollection<UserModel>>(json) ?? new ObservableCollection<UserModel>();
        }

        public static void SaveUsers(ObservableCollection<UserModel> users)
        {
            File.WriteAllText(UsersFile, JsonSerializer.Serialize(users));
        }
    }
}
