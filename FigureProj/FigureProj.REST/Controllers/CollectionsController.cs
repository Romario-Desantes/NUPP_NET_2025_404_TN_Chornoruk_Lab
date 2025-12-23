using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FigureProj.REST.Models;
using FigureProj.Infrastructure;
using FigureProj.Infrastructure.Models;

namespace FigureProj.REST.Controllers
{
    /// <summary>
    /// Контролер для роботи з колекціями фігур
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CollectionsController : ControllerBase
    {
        private readonly FigureContext _context;
        private readonly ILogger<CollectionsController> _logger;

        public CollectionsController(
            FigureContext context,
            ILogger<CollectionsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Отримати всі колекції
        /// </summary>
        /// <returns>Список колекцій</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CollectionDto>>> GetAll()
        {
            _logger.LogInformation("Запит на отримання всіх колекцій");
            
            var collections = await _context.Collections
                .Include(c => c.Figures)
                .ToListAsync();
            
            var dtos = collections.Select(c => MapToDto(c));
            
            return Ok(dtos);
        }

        /// <summary>
        /// Отримати колекцію за ID
        /// </summary>
        /// <param name="id">ID колекції</param>
        /// <returns>Колекція</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CollectionDto>> GetById(int id)
        {
            _logger.LogInformation("Запит на отримання колекції з ID: {Id}", id);
            
            var collection = await _context.Collections
                .Include(c => c.Figures)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (collection == null)
            {
                _logger.LogWarning("Колекцію з ID {Id} не знайдено", id);
                return NotFound($"Колекцію з ID {id} не знайдено");
            }
            
            var dto = MapToDto(collection);
            
            return Ok(dto);
        }

        /// <summary>
        /// Створити нову колекцію
        /// </summary>
        /// <param name="createDto">Дані для створення колекції</param>
        /// <returns>Створена колекція</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CollectionDto>> Create([FromBody] CreateCollectionDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Запит на створення нової колекції: {Name}", createDto.Name);
            
            var collection = new CollectionModel
            {
                Name = createDto.Name,
                Description = createDto.Description,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Collections.Add(collection);
            await _context.SaveChangesAsync();
            
            var dto = MapToDto(collection);
            
            return CreatedAtAction(nameof(GetById), new { id = collection.Id }, dto);
        }

        /// <summary>
        /// Оновити колекцію
        /// </summary>
        /// <param name="id">ID колекції</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <returns>Результат операції</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CollectionDto>> Update(int id, [FromBody] UpdateCollectionDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("ID у URL не співпадає з ID у тілі запиту");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Запит на оновлення колекції з ID: {Id}", id);
            
            var collection = await _context.Collections.FindAsync(id);
            
            if (collection == null)
            {
                _logger.LogWarning("Колекцію з ID {Id} не знайдено", id);
                return NotFound($"Колекцію з ID {id} не знайдено");
            }
            
            collection.Name = updateDto.Name;
            collection.Description = updateDto.Description;
            
            await _context.SaveChangesAsync();
            
            var dto = MapToDto(collection);
            
            return Ok(dto);
        }

        /// <summary>
        /// Видалити колекцію
        /// </summary>
        /// <param name="id">ID колекції</param>
        /// <returns>Результат операції</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Запит на видалення колекції з ID: {Id}", id);
            
            var collection = await _context.Collections.FindAsync(id);
            
            if (collection == null)
            {
                _logger.LogWarning("Колекцію з ID {Id} не знайдено", id);
                return NotFound($"Колекцію з ID {id} не знайдено");
            }
            
            _context.Collections.Remove(collection);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        /// <summary>
        /// Отримати фігури в колекції
        /// </summary>
        /// <param name="id">ID колекції</param>
        /// <returns>Список фігур</returns>
        [HttpGet("{id}/figures")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<FigureDto>>> GetFiguresInCollection(int id)
        {
            _logger.LogInformation("Запит на отримання фігур колекції з ID: {Id}", id);
            
            var collection = await _context.Collections
                .Include(c => c.Figures)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (collection == null)
            {
                _logger.LogWarning("Колекцію з ID {Id} не знайдено", id);
                return NotFound($"Колекцію з ID {id} не знайдено");
            }
            
            var figures = collection.Figures.Select(f => new FigureDto
            {
                Id = f.DomainId,
                Name = f.Name,
                Color = f.Color,
                Area = f.Area,
                Perimeter = f.Perimeter,
                Type = f.GetType().Name.Replace("Model", ""),
                CreatedAt = f.CreatedAt
            });
            
            return Ok(figures);
        }

        // Допоміжні методи

        private CollectionDto MapToDto(CollectionModel collection)
        {
            return new CollectionDto
            {
                Id = collection.Id,
                Name = collection.Name,
                Description = collection.Description,
                CreatedAt = collection.CreatedAt,
                FigureCount = collection.Figures?.Count ?? 0
            };
        }
    }
}

