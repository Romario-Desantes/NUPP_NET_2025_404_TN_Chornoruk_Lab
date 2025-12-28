using System.ComponentModel.DataAnnotations;

namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO для створення нової колекції
    /// </summary>
    public class CreateCollectionDto
    {
        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва повинна бути від 3 до 100 символів")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Опис не повинен перевищувати 500 символів")]
        public string Description { get; set; } = string.Empty;
    }
}

