using System.ComponentModel.DataAnnotations;

namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO для оновлення фігури
    /// </summary>
    public class UpdateFigureDto
    {
        [Required(ErrorMessage = "ID обов'язковий")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва повинна бути від 3 до 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Колір обов'язковий")]
        public string Color { get; set; } = string.Empty;
    }
}

