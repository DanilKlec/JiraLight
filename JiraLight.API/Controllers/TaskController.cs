using AutoMapper;
using JiraLight.API.Controllers.Base;
using JiraLight.API.Domain.Task;
using JiraLight.API.Infrastructure;
using JiraLight.Dto;
using Microsoft.AspNetCore.Mvc;

namespace JiraLight.API.Controllers
{
    public class TaskController : BaseController<TaskCard, TaskCardDto>
    {
        public TaskController(AppDbContext db, IMapper mapper) : base(db, mapper)
        {
        }
    }
}
