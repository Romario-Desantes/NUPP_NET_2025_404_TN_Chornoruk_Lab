namespace FigureProj.REST.Models
{
    /// <summary>
    /// DTO для відповіді при вдалій аутентифікації
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// JWT токен
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Дата закінчення дії токена
        /// </summary>
        public DateTime Expiration { get; set; }

        /// <summary>
        /// Інформація про користувача
        /// </summary>
        public UserInfoDto UserInfo { get; set; } = new UserInfoDto();
    }
}


