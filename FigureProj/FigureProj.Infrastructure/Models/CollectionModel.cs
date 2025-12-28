namespace FigureProj.Infrastructure.Models
{
    /// <summary>
    /// Модель колекції фігур (зв'язок один-до-багатьох)
    /// </summary>
    public class CollectionModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public ICollection<FigureModel> Figures { get; set; } = new List<FigureModel>();
    }
}

