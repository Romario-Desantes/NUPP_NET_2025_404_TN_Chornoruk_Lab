using System.ComponentModel.DataAnnotations;

namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO для входу користувача
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Email користувача
        /// </summary>
        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Пароль
        /// </summary>
        [Required(ErrorMessage = "Пароль обов'язковий")]
        public string Password { get; set; } = string.Empty;
    }
}

