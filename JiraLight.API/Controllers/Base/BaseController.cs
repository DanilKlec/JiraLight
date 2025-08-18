using AutoMapper;
using JiraLight.API.Domain.Base;
using JiraLight.API.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JiraLight.API.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseController<TDomain, TDto> : ControllerBase
        where TDomain : BaseEntity
        where TDto : class
    {
        protected readonly AppDbContext _context;
        protected readonly IMapper _mapper;

        public BaseController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TDto>>> GetAll()
        {
            var items = await _context.Set<TDomain>().ToListAsync();
            return Ok(_mapper.Map<List<TDto>>(items));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TDto>> GetById(int id)
        {
            var entity = await _context.Set<TDomain>().FindAsync(id);
            if (entity == null) return NotFound();
            return Ok(_mapper.Map<TDto>(entity));
        }

        [HttpPost]
        public async Task<ActionResult<TDto>> Create(TDto dto)
        {
            var entity = _mapper.Map<TDomain>(dto);
            _context.Set<TDomain>().Add(entity);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, _mapper.Map<TDto>(entity));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, TDto dto)
        {
            var entity = await _context.Set<TDomain>().FindAsync(id);
            if (entity == null) return NotFound();

            _mapper.Map(dto, entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Set<TDomain>().FindAsync(id);
            if (entity == null) return NotFound();

            _context.Set<TDomain>().Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
