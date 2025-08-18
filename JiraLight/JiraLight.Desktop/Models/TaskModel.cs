namespace JiraLight.Desktop.Models
{
    public class TaskModel
    {
        private static int _nextId = 1;

        public TaskModel()
        {
            Id = _nextId++;
        }

        public int Id { get; private set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public TaskStatus Status { get; set; } // ToDo, InProgress, Done
        public UserModel AssignedUser { get; set; }
        public UserModel CreateUser { get; set; }
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
}
