namespace FigureProj.Infrastructure.Models
{
    public abstract class FigureModel
    {
        public int Id { get; set; }
        public Guid DomainId { get; set; }  // Guid from domain model
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double Area { get; set; }
        public double Perimeter { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public FigureMetadataModel? Metadata { get; set; }
        public int? CollectionId { get; set; }
        public CollectionModel? Collection { get; set; }
        public ICollection<FigureTagModel> FigureTags { get; set; } = new List<FigureTagModel>();
    }
}

