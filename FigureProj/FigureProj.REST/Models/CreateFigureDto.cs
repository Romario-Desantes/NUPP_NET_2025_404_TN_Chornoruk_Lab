using System.ComponentModel.DataAnnotations;

namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO для створення нової фігури
    /// </summary>
    public class CreateFigureDto
    {
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва повинна бути від 3 до 100 символів")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Колір обов'язковий")]
        public string Color { get; set; } = string.Empty;

        [Required(ErrorMessage = "Тип фігури обов'язковий")]
        public string Type { get; set; } = string.Empty;

        // Властивості для різних типів фігур
        public double? Radius { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
        public double? SideA { get; set; }
        public double? SideB { get; set; }
        public double? SideC { get; set; }
    }
}

