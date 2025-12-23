using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FigureProj.Infrastructure.Models;
using FigureProj.REST.Models;

namespace FigureProj.REST.Controllers
{
    /// <summary>
    /// Контролер для аутентифікації та реєстрації
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Реєстрація нового користувача
        /// </summary>
        /// <param name="registerDto">Дані для реєстрації</param>
        /// <returns>Інформація про створеного користувача</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserInfoDto>> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Спроба реєстрації користувача: {UserName}, Email: {Email}", 
                registerDto.UserName, registerDto.Email);

            // Перевірка, чи існує користувач
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Користувач з таким email вже існує" });
            }

            existingUser = await _userManager.FindByNameAsync(registerDto.UserName);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Користувач з таким іменем вже існує" });
            }

            // Створення користувача
            var user = new ApplicationUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                CreatedAt = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Помилка реєстрації: {Errors}", errors);
                return BadRequest(new { message = "Помилка реєстрації", errors = result.Errors });
            }

            // Призначення ролі
            var role = registerDto.Role ?? "Viewer"; // За замовчуванням Viewer
            if (role != "Administrator" && role != "Editor" && role != "Viewer")
            {
                role = "Viewer";
            }

            await _userManager.AddToRoleAsync(user, role);
            _logger.LogInformation("Користувач {UserName} зареєстрований з роллю {Role}", 
                registerDto.UserName, role);

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserInfoDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList()
            });
        }

        /// <summary>
        /// Вхід користувача
        /// </summary>
        /// <param name="loginDto">Дані для входу</param>
        /// <returns>JWT токен та інформація про користувача</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Спроба входу: {Email}", loginDto.Email);

            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
            {
                _logger.LogWarning("Користувача з email {Email} не знайдено", loginDto.Email);
                return Unauthorized(new { message = "Невірний email або пароль" });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Невірний пароль для користувача {Email}", loginDto.Email);
                return Unauthorized(new { message = "Невірний email або пароль" });
            }

            // Оновлення часу останнього входу
            user.LastLogin = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            // Генерація JWT токена
            var token = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("Користувач {Email} успішно увійшов", loginDto.Email);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(
                    int.Parse(_configuration["JwtSettings:ExpirationDays"] ?? "7")),
                UserInfo = new UserInfoDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Roles = roles.ToList()
                }
            });
        }

        /// <summary>
        /// Отримати інформацію про поточного користувача
        /// </summary>
        /// <returns>Інформація про користувача</returns>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserInfoDto>> GetCurrentUser()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized(new { message = "Не вдалося визначити користувача" });
            }

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized(new { message = "Користувача не знайдено" });
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new UserInfoDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList()
            });
        }

        /// <summary>
        /// Генерація JWT токена
        /// </summary>
        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey не налаштований");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("FullName", user.FullName)
            };

            // Додавання ролей до claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(int.Parse(jwtSettings["ExpirationDays"] ?? "7")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}


