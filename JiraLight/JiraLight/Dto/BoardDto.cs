using System.Collections.Generic;

namespace JiraLight.Dto
{
    public class BoardDto : BaseDto
    {
        public string Name { get; set; }
        public IList<TaskCardDto> TaskCards { get; set; }
    }
}
