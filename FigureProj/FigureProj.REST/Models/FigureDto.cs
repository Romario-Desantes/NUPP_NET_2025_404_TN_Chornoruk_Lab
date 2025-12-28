namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO для читання фігури
    /// </summary>
    public class FigureDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public double Area { get; set; }
        public double Perimeter { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

