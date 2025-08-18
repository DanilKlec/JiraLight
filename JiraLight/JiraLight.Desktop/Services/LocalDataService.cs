using JiraLight.Desktop.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace JiraLight.Desktop.Services
{
    internal static class LocalDataService
    {
        private static readonly string TasksFile = "tasks.json";
        private static readonly string UsersFile = "users.json";

        private static ObservableCollection<UserModel> _users;

        public static UserModel CurrentUser { get; private set; }

        static LocalDataService()
        {
            // Загружаем пользователей при старте
            _users = LoadUsers();

            // Если users.json пустой – создаём дефолтных
            if (_users.Count == 0)
            {
                _users = new ObservableCollection<UserModel>
                {
                    new UserModel { Id = 1, Username = "admin", Password = "123", IsAdmin = true, Name = "Admin" },
                    new UserModel { Id = 2, Username = "danil", Password = "321", Name = "Danil" }
                };
                SaveUsers(_users);
            }
        }

        // ✅ Регистрация нового пользователя
        public static bool Register(string username, string password, string name = null)
        {
            if (_users.Any(u => u.Username == username))
                return false; // уже существует

            var newUser = new UserModel
            {
                Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1,
                Username = username,
                Password = password,
                Name = name ?? username
            };

            _users.Add(newUser);
            SaveUsers(_users);
            return true;
        }

        // ✅ Авторизация
        public static bool Login(string username, string password)
        {
            var user = _users.FirstOrDefault(u =>
                u.Username == username && u.Password == password);

            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public static void Logout()
        {
            CurrentUser = null;
        }

        // ✅ Загрузка задач
        public static BoardState LoadTasks()
        {
            if (!File.Exists(TasksFile))
                return new BoardState();

            var json = File.ReadAllText(TasksFile);

            // Попытка десериализации старого формата
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

            // Новый формат
            return JsonSerializer.Deserialize<BoardState>(json) ?? new BoardState();
        }

        public static void SaveTasks(BoardState board)
        {
            var json = JsonSerializer.Serialize(board, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(TasksFile, json);
        }

        // ✅ Загрузка пользователей
        public static ObservableCollection<UserModel> LoadUsers()
        {
            if (!File.Exists(UsersFile)) return new ObservableCollection<UserModel>();
            var json = File.ReadAllText(UsersFile);
            return JsonSerializer.Deserialize<ObservableCollection<UserModel>>(json) ?? new ObservableCollection<UserModel>();
        }

        public static void SaveUsers(ObservableCollection<UserModel> users)
        {
            File.WriteAllText(UsersFile, JsonSerializer.Serialize(users, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
