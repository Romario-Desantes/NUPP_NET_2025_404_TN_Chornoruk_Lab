namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO з інформацією про користувача
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// ID користувача
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Ім'я користувача (логін)
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Повне ім'я
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Ролі користувача
        /// </summary>
        public List<string> Roles { get; set; } = new List<string>();
    }
}

