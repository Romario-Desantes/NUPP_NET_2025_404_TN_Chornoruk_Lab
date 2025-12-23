using System.ComponentModel.DataAnnotations;

namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO для реєстрації нового користувача
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Ім'я користувача (логін)
        /// </summary>
        [Required(ErrorMessage = "Ім'я користувача обов'язкове")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Ім'я користувача має бути від 3 до 50 символів")]
        public string UserName { get; set; } = string.Empty;

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
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути від 6 до 100 символів")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Повне ім'я користувача
        /// </summary>
        [Required(ErrorMessage = "Повне ім'я обов'язкове")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Повне ім'я має бути від 2 до 100 символів")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Роль користувача (Administrator, Editor, Viewer)
        /// </summary>
        public string? Role { get; set; }
    }
}


