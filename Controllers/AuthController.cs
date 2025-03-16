using Microsoft.AspNetCore.Mvc;
using LanguageLearningApp.Data;
using LanguageLearningApp.Domain;
using LanguageLearningApp.Services;
using Microsoft.EntityFrameworkCore;
using LanguageLearningApp.Utilities;

namespace LanguageLearningApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(AppDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            // Kullanıcı adı zaten var mı?
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);
            if (existingUser != null)
                return BadRequest("Username already taken.");

            // Yeni kullanıcı oluştur
            var newUser = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = PasswordHelper.HashPassword(model.Password)
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == model.Username);

            if (user == null)
                return Unauthorized("Invalid username or password.");

            // Parola doğrulama
            bool isValidPass = PasswordHelper.VerifyPassword(model.Password, user.PasswordHash);
            if (!isValidPass)
                return Unauthorized("Invalid username or password.");

            // Token üret
            var token = _tokenService.GenerateToken(user.Id, user.Username);

            return Ok(new { token });
        }
    }

    // Yardımcı DTO sınıfları
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
