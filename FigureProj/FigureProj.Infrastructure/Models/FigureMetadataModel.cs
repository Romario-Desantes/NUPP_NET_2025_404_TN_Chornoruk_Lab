namespace FigureProj.Infrastructure.Models
{
    /// <summary>
    /// Модель метаданих фігури (зв'язок один-до-одного)
    /// </summary>
    public class FigureMetadataModel
    {
        public int Id { get; set; }
        public int FigureId { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime LastModified { get; set; }

        // Navigation property
        public FigureModel Figure { get; set; } = null!;
    }
}

