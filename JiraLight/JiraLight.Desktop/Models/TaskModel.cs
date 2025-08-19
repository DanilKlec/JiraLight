namespace JiraLight.Desktop.Models
{
    public class TaskModel
    {
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
