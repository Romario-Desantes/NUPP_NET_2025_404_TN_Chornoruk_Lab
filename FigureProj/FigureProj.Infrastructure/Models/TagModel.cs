namespace FigureProj.Infrastructure.Models
{
    /// <summary>
    /// Модель тегу для фігур (зв'язок багато-до-багатьох)
    /// </summary>
    public class TagModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public ICollection<FigureTagModel> FigureTags { get; set; } = new List<FigureTagModel>();
    }
}


