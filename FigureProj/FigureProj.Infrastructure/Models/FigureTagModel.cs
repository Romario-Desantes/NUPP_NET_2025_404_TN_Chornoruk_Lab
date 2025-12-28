namespace FigureProj.Infrastructure.Models
{
    /// <summary>
    /// Проміжна таблиця для зв'язку багато-до-багатьох між фігурами та тегами
    /// </summary>
    public class FigureTagModel
    {
        public int FigureId { get; set; }
        public FigureModel Figure { get; set; } = null!;

        public int TagId { get; set; }
        public TagModel Tag { get; set; } = null!;

        public DateTime AssignedAt { get; set; }
    }
}

