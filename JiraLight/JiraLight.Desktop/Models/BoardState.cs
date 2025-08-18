using System.Collections.ObjectModel;

namespace JiraLight.Desktop.Models
{
    public class BoardState
    {
        public ObservableCollection<TaskModel> ToDoTasks { get; set; } = new();
        public ObservableCollection<TaskModel> InProgressTasks { get; set; } = new();
        public ObservableCollection<TaskModel> DoneTasks { get; set; } = new();
    }
}
