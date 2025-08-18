using AutoMapper;
using JiraLight.API.Controllers.Base;
using JiraLight.API.Domain.Boards;
using JiraLight.API.Infrastructure;
using JiraLight.Dto;

namespace JiraLight.API.Controllers
{
    public class BoardController : BaseController<Board, BoardDto>
    {
        public BoardController(AppDbContext context, IMapper mapper) : base(context, mapper)
        {
        }
    }
}
