using Microsoft.AspNetCore.Mvc;
using StudyNotesPlatform.Models;
using StudyNotesPlatform.Services;
using System.Security.Cryptography;
using System.Text;

namespace StudyNotesPlatform.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TokenService _tokenService;

    public AuthController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterModel model)
    {
        if (UserStorage.Users.Any(u => u.Email == model.Email))
        {
            return BadRequest(new AuthResponse { Message = "Пользователь с таким email уже существует" });
        }

        var passwordHash = HashPassword(model.Password);

        var newUser = new User
        {
            Id = UserStorage.Users.Count + 1,
            FullName = model.FullName,
            Email = model.Email,
            PasswordHash = passwordHash,
            University = model.University
        };

        UserStorage.Users.Add(newUser);

        var token = _tokenService.GenerateToken(newUser);

        return Ok(new AuthResponse
        {
            Token = token,
            FullName = newUser.FullName,
            Email = newUser.Email,
            University = newUser.University,
            Message = "Регистрация успешна"
        });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var user = UserStorage.Users.FirstOrDefault(u => u.Email == model.Email);

        if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
        {
            return Unauthorized(new AuthResponse { Message = "Неверный email или пароль" });
        }

        var token = _tokenService.GenerateToken(user);

        return Ok(new AuthResponse
        {
            Token = token,
            FullName = user.FullName,
            Email = user.Email,
            University = user.University,
            Message = "Вход выполнен успешно"
        });
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}