using JiraLight.API.Domain.Base;
using JiraLight.API.Domain.Boards;

namespace JiraLight.API.Domain.Task
{
    public class TaskCard : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int BoardId { get; set; }
        public Board Board { get; set; }
    }
}
