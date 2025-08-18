using JiraLight.API.Domain.Base;
using JiraLight.API.Domain.Task;

namespace JiraLight.API.Domain.Boards
{
    public class Board : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public IList<TaskCard> TaskCards { get; set; } = new List<TaskCard>();
    }
}
