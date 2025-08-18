namespace JiraLight.Dto
{
    public class TaskCardDto : BaseDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int BoardId { get; set; }
        public BoardDto Board { get; set; }
    }
}
