namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO для читання колекції
    /// </summary>
    public class CollectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int FigureCount { get; set; }
    }
}


