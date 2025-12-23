using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FigureProj.REST.Models;
using FigureProj.Common.Services;
using FigureProj.Common.Models.Abstract;
using FigureProj.Common.Models;

namespace FigureProj.REST.Controllers
{
    /// <summary>
    /// Контролер для роботи з фігурами
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FiguresController : ControllerBase
    {
        private readonly ICrudServiceAsync<Figure> _figureService;
        private readonly ILogger<FiguresController> _logger;

        public FiguresController(
            ICrudServiceAsync<Figure> figureService,
            ILogger<FiguresController> logger)
        {
            _figureService = figureService;
            _logger = logger;
        }

        /// <summary>
        /// Отримати всі фігури
        /// </summary>
        /// <returns>Список фігур</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FigureDto>>> GetAll()
        {
            _logger.LogInformation("Запит на отримання всіх фігур");
            
            var figures = await _figureService.ReadAllAsync();
            var dtos = figures.Select(f => MapToDto(f));
            
            return Ok(dtos);
        }

        /// <summary>
        /// Отримати фігури з пагінацією
        /// </summary>
        /// <param name="page">Номер сторінки</param>
        /// <param name="pageSize">Розмір сторінки</param>
        /// <returns>Список фігур</returns>
        [HttpGet("page")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<FigureDto>>> GetPage([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Номер сторінки та розмір повинні бути більше 0");
            }

            _logger.LogInformation("Запит на отримання фігур: сторінка {Page}, розмір {PageSize}", page, pageSize);
            
            var figures = await _figureService.ReadAllAsync(page, pageSize);
            var dtos = figures.Select(f => MapToDto(f));
            
            return Ok(dtos);
        }

        /// <summary>
        /// Отримати фігуру за ID
        /// </summary>
        /// <param name="id">ID фігури</param>
        /// <returns>Фігура</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FigureDto>> GetById(Guid id)
        {
            _logger.LogInformation("Запит на отримання фігури з ID: {Id}", id);
            
            try
            {
                var figure = await _figureService.ReadAsync(id);
                var dto = MapToDto(figure);
                
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Фігуру з ID {Id} не знайдено", id);
                return NotFound($"Фігуру з ID {id} не знайдено");
            }
        }

        /// <summary>
        /// Створити нову фігуру
        /// </summary>
        /// <param name="createDto">Дані для створення фігури</param>
        /// <returns>Створена фігура</returns>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<FigureDto>> Create([FromBody] CreateFigureDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Запит на створення нової фігури: {Name}, тип: {Type}", createDto.Name, createDto.Type);
            
            try
            {
                var figure = CreateFigureFromDto(createDto);
                
                await _figureService.CreateAsync(figure);
                
                var dto = MapToDto(figure);
                
                return CreatedAtAction(nameof(GetById), new { id = figure.Id }, dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні фігури");
                return BadRequest($"Помилка при створенні фігури: {ex.Message}");
            }
        }

        /// <summary>
        /// Оновити фігуру
        /// </summary>
        /// <param name="id">ID фігури</param>
        /// <param name="updateDto">Дані для оновлення</param>
        /// <returns>Результат операції</returns>
        [Authorize(Roles = "Editor,Administrator")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<FigureDto>> Update(Guid id, [FromBody] UpdateFigureDto updateDto)
        {
            if (id != updateDto.Id)
            {
                return BadRequest("ID у URL не співпадає з ID у тілі запиту");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Запит на оновлення фігури з ID: {Id}", id);
            
            try
            {
                var figure = await _figureService.ReadAsync(id);
                
                figure.Name = updateDto.Name;
                figure.Color = updateDto.Color;
                
                await _figureService.UpdateAsync(figure);
                
                var dto = MapToDto(figure);
                
                return Ok(dto);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Фігуру з ID {Id} не знайдено", id);
                return NotFound($"Фігуру з ID {id} не знайдено");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні фігури");
                return BadRequest($"Помилка при оновленні фігури: {ex.Message}");
            }
        }

        /// <summary>
        /// Видалити фігуру
        /// </summary>
        /// <param name="id">ID фігури</param>
        /// <returns>Результат операції</returns>
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Запит на видалення фігури з ID: {Id}", id);
            
            try
            {
                var figure = await _figureService.ReadAsync(id);
                await _figureService.RemoveAsync(figure);
                
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Фігуру з ID {Id} не знайдено", id);
                return NotFound($"Фігуру з ID {id} не знайдено");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні фігури");
                return BadRequest($"Помилка при видаленні фігури: {ex.Message}");
            }
        }

        // Допоміжні методи для маппінгу

        private FigureDto MapToDto(Figure figure)
        {
            return new FigureDto
            {
                Id = figure.Id,
                Name = figure.Name,
                Color = figure.Color,
                Area = figure.Area,
                Perimeter = figure.Perimeter,
                Type = figure.GetType().Name,
                CreatedAt = DateTime.UtcNow
            };
        }

        private Figure CreateFigureFromDto(CreateFigureDto dto)
        {
            return dto.Type.ToLower() switch
            {
                "circle" or "коло" => new Circle(
                    dto.Radius ?? throw new ArgumentException("Радіус обов'язковий для кола"),
                    dto.Name,
                    dto.Color),
                
                "rectangle" or "прямокутник" => new Rectangle(
                    dto.Height ?? throw new ArgumentException("Висота обов'язкова для прямокутника"),
                    dto.Width ?? throw new ArgumentException("Ширина обов'язкова для прямокутника"),
                    dto.Name,
                    dto.Color),
                
                "square" or "квадрат" => new Square(
                    dto.Width ?? throw new ArgumentException("Сторона обов'язкова для квадрата"),
                    dto.Name,
                    dto.Color),
                
                "triangle" or "трикутник" => new Triangle(
                    dto.SideA ?? throw new ArgumentException("Сторона A обов'язкова для трикутника"),
                    dto.SideB ?? throw new ArgumentException("Сторона B обов'язкова для трикутника"),
                    dto.SideC ?? throw new ArgumentException("Сторона C обов'язкова для трикутника"),
                    dto.Name,
                    dto.Color),
                
                _ => throw new ArgumentException($"Невідомий тип фігури: {dto.Type}")
            };
        }
    }
}

