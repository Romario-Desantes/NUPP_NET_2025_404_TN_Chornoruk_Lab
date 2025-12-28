using System.ComponentModel.DataAnnotations;

namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO для оновлення колекції
    /// </summary>
    public class UpdateCollectionDto
    {
        [Required(ErrorMessage = "ID обов'язковий")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва обов'язкова")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Назва повинна бути від 3 до 100 символів")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Опис не повинен перевищувати 500 символів")]
        public string Description { get; set; } = string.Empty;
    }
}


