using Microsoft.AspNetCore.Identity;

namespace FigureProj.Infrastructure.Models
{
    /// <summary>
    /// Сутність користувача застосунку, розширює IdentityUser
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Повне ім'я користувача
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Дата створення облікового запису
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Дата останнього входу
        /// </summary>
        public DateTime? LastLogin { get; set; }
    }
}


